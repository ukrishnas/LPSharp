// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Collections.Generic;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents the model of a linear program.
    /// </summary>
    public class LPModel
    {
        /// <summary>
        /// The rows of the constraint matrix.
        /// </summary>
        private readonly Dictionary<string, Row> rows;

        /// <summary>
        /// The columns of the constraint matrix.
        /// </summary>
        private readonly Dictionary<string, SparseVector<double>> columns;

        /// <summary>
        /// The right hand side column vectors.
        /// </summary>
        private readonly Dictionary<string, SparseVector<double>> rhs;

        /// <summary>
        /// The bounds on variables.
        /// </summary>
        private readonly Dictionary<string, SparseVector<Bound>> bounds;

        /// <summary>
        /// The ranges vectors.
        /// </summary>
        private readonly Dictionary<string, SparseVector<double>> ranges;

        /// <summary>
        /// If true, keep a second copy of the coefficients in column vectors.
        /// </summary>
        private readonly bool keepColumns;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPModel"/> class.
        /// </summary>
        /// <param name="keepColumns">If true, also maintain column vectors.</param>
        public LPModel(bool keepColumns = false)
        {
            this.rows = new Dictionary<string, Row>();
            this.columns = new Dictionary<string, SparseVector<double>>();
            this.rhs = new Dictionary<string, SparseVector<double>>();
            this.bounds = new Dictionary<string, SparseVector<Bound>>();
            this.ranges = new Dictionary<string, SparseVector<double>>();
            this.keepColumns = keepColumns;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Name={this.Name} Rows={this.rows.Count} Columns={this.columns.Count} " +
                $"RHS={this.rhs.Count} Bounds={this.bounds.Count} Ranges={this.ranges.Count}";
        }

        /// <summary>
        /// Gets or sets the name of the problem.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The row name of the objective.
        /// </summary>
        public string Objective { get; private set; }

        /// <summary>
        /// Adds a row.
        /// </summary>
        /// <param name="name">The row name.</param>
        /// <param name="type">The row type.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool AddRow(string name, MpsRow type)
        {
            if (this.rows.ContainsKey(name))
            {
                return false;
            }

            this.rows.Add(name, new Row(name, type));

            if (type == MpsRow.NoRestriction && this.Objective == null)
            {
                this.Objective = name;
            }

            return true;
        }

        /// <summary>
        /// Adds a entry in the coefficient matrix.
        /// </summary>
        /// <param name="columnName">The column or variable name.</param>
        /// <param name="rowName">The row name.</param>
        /// <param name="value">The value.</param>
        /// <returns>True on success, false on faiilure.</returns>
        public bool AddCoefficient(string columnName, string rowName, double value)
        {
            if (!this.rows.TryGetValue(rowName, out Row row))
            {
                return false;
            }

            if (row.Coefficients.ContainsKey(columnName))
            {
                return false;
            }

            if (!this.columns.TryGetValue(columnName, out SparseVector<double> column))
            {
                if (this.keepColumns)
                {
                    column = new SparseVector<double>();
                }
                
                this.columns.Add(columnName, column);
            }

            row.Coefficients.Add(columnName, value);

            if (column != null)
            {
                column[rowName] = value;
            }

            return true;
        }

        /// <summary>
        /// Adds an element to a right hand side vector.
        /// </summary>
        /// <param name="rhsName">The right hand side name.</param>
        /// <param name="rowName">The row name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool AddRhs(string rhsName, string rowName, double value)
        {
            if (!this.rhs.TryGetValue(rhsName, out SparseVector<double> rhs))
            {
                rhs = new SparseVector<double>();
                this.rhs.Add(rhsName, rhs);
            }

            if (rhs.ContainsKey(rowName))
            {
                return false;
            }

            rhs.Add(rowName, value);
            return true;
        }

        /// <summary>
        /// Adds a bound on a variable.
        /// </summary>
        /// <param name="boundName">The bounds name.</param>
        /// <param name="columnName">The column or variable name.</param>
        /// <param name="type">The bound type.</param>
        /// <param name="value"/>The bound value.</param>
        /// <returns>True on success, false otherwise.</returns>
        public bool AddBound(string boundsName, string columnName, MpsBound type, double value)
        {
            if (!this.bounds.TryGetValue(boundsName, out SparseVector<Bound> bounds))
            {
                bounds = new SparseVector<Bound>();
                this.bounds.Add(boundsName, bounds);
            }

            if (!bounds.TryGetValue(columnName, out Bound bound))
            {
                bound = new Bound(type, value);
                bounds.Add(columnName, bound);
            }
            else
            {
                if (bound.Single)
                {
                    bound.Add(type, value);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds an element to a range vector.
        /// </summary>
        /// <param name="rangesName">The ranges name.</param>
        /// <param name="rowName">The row name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool AddRange(string rangesName, string rowName, double value)
        {
            if (!this.ranges.TryGetValue(rangesName, out SparseVector<double> ranges))
            {
                ranges = new SparseVector<double>();
                this.ranges.Add(rangesName, ranges);
            }

            if (ranges.ContainsKey(rowName))
            {
                return false;
            }

            ranges.Add(rowName, value);
            return true;
        }

        /// <summary>
        /// Represents a sparse vector as a dictionary of element name and value.
        /// </summary>
        /// <typeparam name="T">The type of element in the vector.</typeparam>
        private class SparseVector<T> : Dictionary<string, T>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SparseVector"/> class.
            /// </summary>
            public SparseVector()
                : base()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SparseVector"/> class.
            /// </summary>
            /// <param name="capacity"></param>
            public SparseVector(int capacity)
                : base(capacity)
            {
            }
        }

        private class Row
        {
            /// <summary>
            /// The row name.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// The row type.
            /// </summary>
            public readonly MpsRow Type;

            /// <summary>
            /// The row coefficients keyed by the variable name.
            /// </summary>
            public readonly SparseVector<double> Coefficients;

            /// <summary>
            /// Initializes a new instance of the <see cref="Row"/> class.
            /// </summary>
            /// <param name="name">The row name.</param>
            /// <param name="type">The row type.</param>
            public Row(string name, MpsRow type)
            {
                this.Name = name;
                this.Type = type;
                this.Coefficients = new SparseVector<double>();
            }
        }

        /// <summary>
        /// Represents a bound on a variable.
        /// </summary>
        private class Bound
        {
            /// <summary>
            /// If true, only first bound is valid.
            /// </summary>
            public bool Single;

            /// <summary>
            /// The first bound type.
            /// </summary>
            public MpsBound Type1;

            /// <summary>
            /// The first bound value.
            /// </summary>
            public double Value1;

            /// <summary>
            /// The second bound type.
            /// </summary>
            public MpsBound Type2;

            /// <summary>
            /// The second bound value.
            /// </summary>
            public double Value2;

            /// <summary>
            /// Initializes a new instance of the <see cref="Bound"/> class.
            /// </summary>
            /// <param name="type">The first bound type.</param>
            /// <param name="value">The first bound value.</param>
            public Bound(MpsBound type, double value)
            {
                this.Type1 = type;
                this.Value1 = value;
                this.Single = true;
            }

            /// <summary>
            /// Adds the second bound.
            /// </summary>
            /// <param name="type">The second bound type.</param>
            /// <param name="value">The second bound value.</param>
            public void Add(MpsBound type, double value)
            {
                this.Single = false;
                this.Type2 = type;
                this.Value2 = value;
            }
        }
    }
}
