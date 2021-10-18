// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents the model of a linear program.
    /// </summary>
    public class LPModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LPModel"/> class.
        /// </summary>
        public LPModel()
        {
            this.A = new SparseMatrix<string, double>();
            this.RowTypes = new SparseVector<string, MpsRow>();
            this.B = new SparseMatrix<string, double>();
            this.L = new SparseMatrix<string, double>();
            this.U = new SparseMatrix<string, double>();
            this.R = new SparseMatrix<string, double>();
        }

        /// <summary>
        /// Gets or sets the name of the problem.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the row name of the objective.
        /// </summary>
        public string Objective { get; private set; }

        /// <summary>
        /// Gets the coefficient matrix organized by row vectors.
        /// </summary>
        public SparseMatrix<string, double> A { get; }

        /// <summary>
        /// Gets the inequality types for each row in the coefficient matrix.
        /// </summary>
        public SparseVector<string, MpsRow> RowTypes { get; }

        /// <summary>
        /// Gets the right hand side vectors organized as a matrix where each row is an RHS column vector
        /// for a RHS name.
        /// </summary>
        public SparseMatrix<string, double> B { get; }

        /// <summary>
        /// Gets the lower bounds on variables organized as a matrix of row vectors, one row per bounds name.
        /// Unspecified lower bound is zero.
        /// </summary>
        public SparseMatrix<string, double> L { get; }

        /// <summary>
        /// Gets the upper bounds on variables organized as a matrix of row vectors, one row per bounds name.
        /// Unspecified upper bound is positive infinity.
        /// </summary>
        public SparseMatrix<string, double> U { get;  }

        /// <summary>
        /// Gets the ranges for the right hand side vector organized as a matrix where each row is the range
        /// vector for a range name.
        /// </summary>
        public SparseMatrix<string, double> R { get; }

        /// <summary>
        /// Gets the collection of right hand side names.
        /// </summary>
        public IReadOnlyList<string> RhsNames => this.B.RowCount > 0 ? this.B.Indices.ToList() : null;

        /// <summary>
        /// Gets the collection of bounds names.
        /// </summary>
        public IReadOnlyList<string> BoundsNames => this.L.RowCount > 0 ? this.L.Indices.ToList() : null;

        /// <summary>
        /// Gets the collection of range names.
        /// </summary>
        public IReadOnlyList<string> RangeNames => this.R.RowCount > 0 ? this.R.Indices.ToList() : null;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Name={this.Name} Obj={this.Objective} " +
                $"A={this.A} RHS={this.B} Lower={this.L} Upper={this.U} " +
                $"RowTypes={this.RowTypes.Count} Ranges={this.R}";
        }

        /// <summary>
        /// Sets the objective to the first no restriction row.
        /// </summary>
        /// <returns>The objective index if found, else null.</returns>
        public string SetObjective()
        {
            foreach (var index in this.RowTypes.Indices)
            {
                if (this.RowTypes[index] == MpsRow.NoRestriction)
                {
                    this.Objective = index;
                    return index;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the bound on a variable.
        /// </summary>
        /// <param name="boundsName">The bounds name.</param>
        /// <param name="columnName">The column or variable name.</param>
        /// <param name="type">The bound type.</param>
        /// <param name="value">The bound value.</param>
        /// <returns>True on success, false if not supported.</returns>
        public bool SetBound(string boundsName, string columnName, MpsBound type, double value)
        {
            switch (type)
            {
                case MpsBound.Lower:
                    this.L[boundsName, columnName] = value;
                    if (!this.U.Has(boundsName, columnName))
                    {
                        this.U[boundsName, columnName] = double.PositiveInfinity;
                    }

                    break;

                case MpsBound.Upper:
                    this.U[boundsName, columnName] = value;
                    if (!this.L.Has(boundsName, columnName))
                    {
                        this.L[boundsName, columnName] = 0;
                    }

                    break;

                case MpsBound.Fixed:
                    this.L[boundsName, columnName] = this.U[boundsName, columnName] = value;
                    break;

                case MpsBound.Free:
                    this.L[boundsName, columnName] = double.NegativeInfinity;
                    this.U[boundsName, columnName] = double.PositiveInfinity;
                    break;

                case MpsBound.MinusInfinity:
                    this.L[boundsName, columnName] = double.NegativeInfinity;
                    if (!this.U.Has(boundsName, columnName))
                    {
                        this.U[boundsName, columnName] = double.PositiveInfinity;
                    }

                    break;

                case MpsBound.PlusInfinity:
                    this.U[boundsName, columnName] = double.PositiveInfinity;
                    if (!this.L.Has(boundsName, columnName))
                    {
                        this.L[boundsName, columnName] = 0;
                    }

                    break;

                default:
                    // The rest are not supported.
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the lower and upper bounds for variables.
        /// </summary>
        /// <param name="boundsName">The bounds name.</param>
        /// <param name="lowerBound">The lower bound row vector (output).</param>
        /// <param name="upperBound">The upper bound row vector (output).</param>
        /// <param name="defaultLowerBound">The default lower bound.</param>
        /// <param name="defaultUpperBound">The default upper bound.</param>
        /// <returns>True if bounds were used, false if bounds are default values.</returns>
        public bool GetVariableBounds(
            string boundsName,
            out SparseVector<string, double> lowerBound,
            out SparseVector<string, double> upperBound,
            double defaultLowerBound = 0,
            double defaultUpperBound = double.PositiveInfinity)
        {
            lowerBound = new SparseVector<string, double> { Default = defaultLowerBound };
            upperBound = new SparseVector<string, double> { Default = defaultUpperBound };

            // If no bounds name is given, use the first bounds name.
            if (string.IsNullOrEmpty(boundsName))
            {
                boundsName = this.BoundsNames?[0];
            }

            if (!this.L.Has(boundsName) || !this.U.Has(boundsName))
            {
                return false;
            }

            var lower = this.L[boundsName];
            var upper = this.U[boundsName];
            foreach (var colIndex in this.A.ColumnIndices)
            {
                if (lower.Has(colIndex))
                {
                    lowerBound[colIndex] = lower[colIndex];
                }

                if (upper.Has(colIndex))
                {
                    upperBound[colIndex] = upper[colIndex];
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the lower and upper limit right hand side vectors.
        /// </summary>
        /// <param name="rhsName">The right hand side name.</param>
        /// <param name="rangesName">The ranges name or null if no range should be used.</param>
        /// <param name="lowerLimit">The lower limit right hand side vector (output).</param>
        /// <param name="upperLimit">The upper limit right hand side vector (output).</param>
        /// <param name="defaultLowerLimit">The default lower limit.</param>
        /// <param name="defaultUpperLimit">The default upper limit.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool GetRhsLimits(
            string rhsName,
            string rangesName,
            out SparseVector<string, double> lowerLimit,
            out SparseVector<string, double> upperLimit,
            double defaultLowerLimit = double.NegativeInfinity,
            double defaultUpperLimit = double.PositiveInfinity)
        {
            lowerLimit = new SparseVector<string, double> { Default = defaultLowerLimit };
            upperLimit = new SparseVector<string, double> { Default = defaultUpperLimit };

            if (string.IsNullOrEmpty(rhsName))
            {
                rhsName = this.RhsNames?[0];
            }

            if (!this.B.Has(rhsName))
            {
                return false;
            }

            var rangeVector = this.R[rangesName];
            foreach (var rowIndex in this.B[rhsName].Indices)
            {
                var rhs = this.B[rhsName, rowIndex];

                var rowType = this.RowTypes[rowIndex];
                if (rowType == MpsRow.NoRestriction)
                {
                    continue;
                }
                else if (rowType == MpsRow.GreaterOrEqual)
                {
                    var range = rangeVector != null && rangeVector.Has(rowIndex) ? rangeVector[rowIndex] : double.PositiveInfinity;
                    lowerLimit[rowIndex] = rhs;
                    upperLimit[rowIndex] = rhs + Math.Abs(range);
                }
                else if (rowType == MpsRow.LessOrEqual)
                {
                    var range = rangeVector != null && rangeVector.Has(rowIndex) ? rangeVector[rowIndex] : double.PositiveInfinity;
                    lowerLimit[rowIndex] = rhs - Math.Abs(range);
                    upperLimit[rowIndex] = rhs;
                }
                else if (rowType == MpsRow.Equal)
                {
                    var range = rangeVector != null && rangeVector.Has(rowIndex) ? rangeVector[rowIndex] : 0;
                    if (range >= 0)
                    {
                        lowerLimit[rowIndex] = rhs;
                        upperLimit[rowIndex] = rhs + range;
                    }
                    else
                    {
                        lowerLimit[rowIndex] = rhs + range;
                        upperLimit[rowIndex] = rhs;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the model is valid.
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        public bool IsValid()
        {
            // Check if model has an objective.
            if (string.IsNullOrEmpty(this.Objective))
            {
                return false;
            }

            // Check if model has a right hand side.
            if (this.RhsNames == null)
            {
                return false;
            }

            // Check if lower bound is less than or equal to the upper bound.
            if (this.BoundsNames != null)
            {
                foreach (var boundsName in this.BoundsNames)
                {
                    this.GetVariableBounds(
                        boundsName,
                        out SparseVector<string, double> lowerBound,
                        out SparseVector<string, double> upperBound);
                    foreach (var index in lowerBound.Indices)
                    {
                        if (lowerBound[index] > upperBound[index])
                        {
                            return false;
                        }
                    }
                }
            }

            // Check if right hand side lower limit is less than or equal to the upper limit.
            foreach (var rhsName in this.RhsNames)
            {
                this.GetRhsLimits(
                    rhsName,
                    null,
                    out SparseVector<string, double> lowerLimit,
                    out SparseVector<string, double> upperLimit);
                foreach (var index in lowerLimit.Indices)
                {
                    if (lowerLimit[index] > upperLimit[index])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
