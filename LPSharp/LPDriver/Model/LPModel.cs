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
    using System.Text;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents the model of a linear program. The model is based on data records in an MPS file.
    /// </summary>
    public class LPModel
    {
        /// <summary>
        /// The default lower and upper bounds for variables.
        /// </summary>
        private Tuple<double, double> defaultVariableBound;

        /// <summary>
        /// The default lower and upper bounds for constraints.
        /// </summary>
        private Tuple<double, double> defaultConstraintBound;

        /// <summary>
        /// The selected bound name.
        /// </summary>
        private string selectedBoundName;

        /// <summary>
        /// The selected right hand side name.
        /// </summary>
        private string selectedRhsName;

        /// <summary>
        /// The selected range name.
        /// </summary>
        private string selectedRangeName;

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

            // Default variable and constraint bounds which the user can change if desired.
            this.defaultVariableBound = new(0, double.PositiveInfinity);
            this.defaultConstraintBound = new(double.NegativeInfinity, double.PositiveInfinity);
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
        /// Gets the constraint matrix organized by row vectors.
        /// </summary>
        public SparseMatrix<string, double> A { get; }

        /// <summary>
        /// Gets the inequality types for each row in the constraint matrix.
        /// </summary>
        public SparseVector<string, MpsRow> RowTypes { get; }

        /// <summary>
        /// Gets the right hand side vectors organized as a matrix where each row is an RHS column vector
        /// for a RHS name.
        /// </summary>
        public SparseMatrix<string, double> B { get; }

        /// <summary>
        /// Gets the lower bounds on variables organized as a matrix of row vectors, one row per bounds name.
        /// </summary>
        public SparseMatrix<string, double> L { get; }

        /// <summary>
        /// Gets the upper bounds on variables organized as a matrix of row vectors, one row per bounds name.
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
        public IReadOnlyList<string> BoundNames => this.L.RowCount > 0 ? this.L.Indices.ToList() : null;

        /// <summary>
        /// Gets the collection of range names.
        /// </summary>
        public IReadOnlyList<string> RangeNames => this.R.RowCount > 0 ? this.R.Indices.ToList() : null;

        /// <summary>
        /// Gets or sets the default variable bound.
        /// </summary>
        public Tuple<double, double> DefaultVariableBound
        {
            get => this.defaultVariableBound;
            set
            {
                if (value.Item1 <= value.Item2)
                {
                    this.defaultVariableBound = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default constraint bound.
        /// </summary>
        public Tuple<double, double> DefaultConstraintBound
        {
            get => this.defaultConstraintBound;
            set
            {
                if (value.Item1 <= value.Item2)
                {
                    this.defaultConstraintBound = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected bound name.
        /// </summary>
        public string SelectedBoundName
        {
            get => this.selectedBoundName;
            set
            {
                if (this.L.Has(value))
                {
                    this.selectedBoundName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected right hand side name.
        /// </summary>
        public string SelectedRhsName
        {
            get => this.selectedRhsName;
            set
            {
                if (this.B.Has(value))
                {
                    this.selectedRhsName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected range name.
        /// </summary>
        public string SelectedRangeName
        {
            get => this.selectedRangeName;
            set
            {
                if (this.R.Has(value))
                {
                    this.selectedRangeName = value;
                }
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Name={this.Name} Obj={this.Objective} A={this.A} RHS={this.B} ");
            sb.Append($"RowTypes={this.RowTypes.Count} Lower={this.L} Upper={this.U} Ranges={this.R} ");
            sb.Append($"DefaultVarBound=({this.defaultVariableBound.Item1},{this.defaultVariableBound.Item2}) ");
            sb.Append($"DefaultConstraintBound=({this.defaultConstraintBound.Item1},{this.defaultConstraintBound.Item2}) ");
            sb.Append($"SelectedRhs={this.selectedRhsName}, SelectedBound={this.SelectedBoundName}, SelectedRange={this.SelectedRangeName}");
            return sb.ToString();
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
        /// <param name="boundName">The bound name.</param>
        /// <param name="columnName">The column or variable name.</param>
        /// <param name="type">The bound type.</param>
        /// <param name="value">The bound value.</param>
        /// <returns>True on success, false if not supported.</returns>
        public bool SetBound(string boundName, string columnName, MpsBound type, double value)
        {
            var defaultLower = this.defaultVariableBound.Item1;
            var defaultUpper = this.defaultVariableBound.Item2;

            switch (type)
            {
                case MpsBound.Lower:
                    this.L[boundName, columnName] = value;
                    if (!this.U.Has(boundName, columnName))
                    {
                        this.U[boundName, columnName] = defaultUpper;
                    }

                    break;

                case MpsBound.Upper:
                    this.U[boundName, columnName] = value;
                    if (!this.L.Has(boundName, columnName))
                    {
                        this.L[boundName, columnName] = defaultLower;
                    }

                    break;

                case MpsBound.Fixed:
                    this.L[boundName, columnName] = this.U[boundName, columnName] = value;
                    break;

                case MpsBound.Free:
                    this.L[boundName, columnName] = defaultLower;
                    this.U[boundName, columnName] = defaultUpper;
                    break;

                case MpsBound.MinusInfinity:
                    this.L[boundName, columnName] = double.NegativeInfinity;
                    if (!this.U.Has(boundName, columnName))
                    {
                        this.U[boundName, columnName] = defaultUpper;
                    }

                    break;

                case MpsBound.PlusInfinity:
                    this.U[boundName, columnName] = double.PositiveInfinity;
                    if (!this.L.Has(boundName, columnName))
                    {
                        this.L[boundName, columnName] = defaultLower;
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
        /// <param name="lowerBound">The lower bound for variables (output).</param>
        /// <param name="upperBound">The upper bound for variables (output).</param>
        public void GetVariableBounds(
            out SparseVector<string, double> lowerBound,
            out SparseVector<string, double> upperBound)
        {
            var defaultLower = this.defaultVariableBound.Item1;
            var defaultUpper = this.defaultVariableBound.Item2;

            lowerBound = new SparseVector<string, double>(defaultLower);
            upperBound = new SparseVector<string, double>(defaultUpper);

            // Use the selected or first bound name.
            var boundName = this.selectedBoundName ?? this.BoundNames?[0];

            // If no bound is present, then there is nothing more to do. The lower and upper bound
            // vectors have the default values.
            if (!this.L.Has(boundName) || !this.U.Has(boundName))
            {
                return;
            }

            var lower = this.L[boundName];
            var upper = this.U[boundName];
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
        }

        /// <summary>
        /// Gets the lower and upper bounds for constraints.
        /// </summary>
        /// <param name="lowerBound">The lower bound for constraints (output).</param>
        /// <param name="upperBound">The upper bound for constraints (output).</param>
        public void GetConstraintBounds(
            out SparseVector<string, double> lowerBound,
            out SparseVector<string, double> upperBound)
        {
            var defaultLower = this.defaultConstraintBound.Item1;
            var defaultUpper = this.defaultConstraintBound.Item2;

            lowerBound = new SparseVector<string, double>(defaultLower);
            upperBound = new SparseVector<string, double>(defaultUpper);

            // Use the selected or first right hand side name.
            var rhsName = this.selectedRhsName ?? this.RhsNames?[0];

            // If the right hand side is not found, then use a zero vector.
            SparseVector<string, double> rhsVector;
            if (this.B.Has(rhsName))
            {
                rhsVector = this.B[rhsName];
            }
            else
            {
                rhsVector = new SparseVector<string, double>(0);
            }

            foreach (var rowIndex in this.A.RowIndices)
            {
                var rowType = this.RowTypes[rowIndex];
                if (rowType == MpsRow.NoRestriction)
                {
                    continue;
                }

                // If the right hand side vector does not have an element for the row index,
                // it will return zero, just like coefficients in the constraint matrix.
                var rhs = rhsVector[rowIndex];

                if (rowType == MpsRow.GreaterOrEqual)
                {
                    lowerBound[rowIndex] = rhs;
                }
                else if (rowType == MpsRow.LessOrEqual)
                {
                    upperBound[rowIndex] = rhs;
                }
                else if (rowType == MpsRow.Equal)
                {
                    lowerBound[rowIndex] = upperBound[rowIndex] = rhs;
                }
            }
        }

        /// <summary>
        /// Gets the lower and upper bounds for constraints based on a range.
        /// </summary>
        /// <param name="lowerBound">The lower bound for constraints.</param>
        /// <param name="upperBound">The upper bound for constraints.</param>
        public void UpdateConstraintBoundsWithRange(
            SparseVector<string, double> lowerBound,
            SparseVector<string, double> upperBound)
        {
            // Use the selected or first range name.
            var rangeName = this.selectedRangeName ?? this.RangeNames?[0];

            if (!this.R.Has(rangeName))
            {
                return;
            }

            var rangeVector = this.R[rangeName];

            foreach (var rowIndex in this.A.RowIndices)
            {
                var rowType = this.RowTypes[rowIndex];
                if (rowType == MpsRow.NoRestriction)
                {
                    continue;
                }

                // If the row index is not specified in the range, the default value will be zero.
                // A zero change does not have any affect on the constraint bounds.
                if (!rangeVector.Has(rowIndex))
                {
                    continue;
                }

                var range = rangeVector[rowIndex];

                if (lowerBound[rowIndex] == upperBound[rowIndex])
                {
                    if (range >= 0)
                    {
                        upperBound[rowIndex] += range;
                    }
                    else
                    {
                        lowerBound[rowIndex] += range;
                    }
                }
                else if (lowerBound[rowIndex] == this.defaultConstraintBound.Item1)
                {
                    lowerBound[rowIndex] = upperBound[rowIndex] - Math.Abs(range);
                }
                else if (upperBound[rowIndex] == this.defaultConstraintBound.Item2)
                {
                    upperBound[rowIndex] = lowerBound[rowIndex] + Math.Abs(range);
                }
            }
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

            // Check if right hand side lower bound is less than or equal to the upper bound.
            if (this.RhsNames != null)
            {
                this.GetConstraintBounds(
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

            // Check if lower bound is less than or equal to the upper bound.
            if (this.BoundNames != null)
            {
                this.GetVariableBounds(
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

            return true;
        }
    }
}
