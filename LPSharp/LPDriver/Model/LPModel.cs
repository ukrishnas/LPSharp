// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
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
                case MpsBound.LO:
                    this.L[boundsName, columnName] = value;
                    break;

                case MpsBound.UP:
                    this.U[boundsName, columnName] = value;
                    break;

                case MpsBound.FX:
                    this.L[boundsName, columnName] = this.U[boundsName, columnName] = value;
                    break;

                case MpsBound.FR:
                    this.L[boundsName, columnName] = double.NegativeInfinity;
                    this.U[boundsName, columnName] = double.PositiveInfinity;
                    break;

                case MpsBound.MI:
                    // Upper bound is \infty, or the value set by Upper, PL, or UI.
                    this.L[boundsName, columnName] = double.NegativeInfinity;
                    break;

                case MpsBound.PL:
                    // The lower bound if 0, or the value set by LO, MI, or LI.
                    this.U[boundsName, columnName] = double.PositiveInfinity;
                    break;

                case MpsBound.LI:
                    this.L[boundsName, columnName] = Math.Round(value);
                    break;

                case MpsBound.UI:
                    this.U[boundsName, columnName] = Math.Round(value);
                    break;

                case MpsBound.BV:
                    this.L[boundsName, columnName] = 0;
                    this.U[boundsName, columnName] = 1;
                    break;

                default:
                    // The rest are not supported.
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the lower and upper limit for an RHS element.
        /// </summary>
        /// <param name="rangesName">The ranges name.</param>
        /// <param name="rhsName">The RHS name.</param>
        /// <param name="rowName">The row name.</param>
        /// <param name="lowerLimit">Output parameter for the lower limit of b(i).</param>
        /// <param name="upperLimit">Output parameter for the upper limit of b(i).</param>
        /// <returns>True on success, false on failure.</returns>
        public bool GetRhsRange(string rangesName, string rhsName, string rowName, out double lowerLimit, out double upperLimit)
        {
            lowerLimit = 0;
            upperLimit = double.PositiveInfinity;

            if (!this.R.Has(rangesName) || !this.B.Has(rhsName) || !this.RowTypes.Has(rowName))
            {
                return false;
            }

            var rowType = this.RowTypes[rowName];
            if (rowType == MpsRow.NoRestriction)
            {
                return false;
            }

            var range = this.R[rangesName, rowName];
            var rhs = this.B[rhsName, rowName];

            if (rowType == MpsRow.GreaterOrEqual)
            {
                lowerLimit = rhs;
                upperLimit = rhs + Math.Abs(range);
            }
            else if (rowType == MpsRow.LessOrEqual)
            {
                lowerLimit = rhs - Math.Abs(range);
                upperLimit = rhs;
            }
            else if (rowType == MpsRow.Equal)
            {
                if (range >= 0)
                {
                    lowerLimit = rhs;
                    upperLimit = rhs + range;
                }
                else
                {
                    lowerLimit = rhs + range;
                    upperLimit = rhs;
                }
            }

            return true;
        }
    }
}
