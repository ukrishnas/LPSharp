// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPModelComparer.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using LPSharp.LPDriver.Contract;

    /// <summary>
    /// Compares two LP models and shows the differences.
    /// </summary>
    public class LPModelComparer : IComparer
    {
        /// <summary>
        /// The errors while reading the file.
        /// </summary>
        private readonly List<string> differences;

        /// <summary>
        /// The set of row indices to ignore. This is used to ignore the objective row indices
        /// since they can have different names in each model.
        /// </summary>
        private readonly HashSet<string> ignoreRowIndices;

        /// <summary>
        /// The tolerance for double precision comparison.
        /// </summary>
        private double tolerance;

        /// <summary>
        /// The first model's objective row index.
        /// </summary>
        private string firstObjective;

        /// <summary>
        /// The second model's objective row index.
        /// </summary>
        private string secondObjective;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPModelComparer"/> class.
        /// </summary>
        public LPModelComparer()
        {
            this.differences = new List<string>();
            this.ignoreRowIndices = new HashSet<string>();
            this.Tolerance = LPConstant.DefaultTolerance;
        }

        /// <summary>
        /// Gets or sets the tolerance for double precision comparison.
        /// </summary>
        public double Tolerance
        {
            get => this.tolerance;
            set => this.tolerance = value == 0 ? LPConstant.DefaultTolerance : Math.Abs(value);
        }

        /// <summary>
        /// Gets the enumeration of differences in the two LP models.
        /// </summary>
        public IReadOnlyList<string> Differences => this.differences;

        /// <inheritdoc />
        public int Compare(object x, object y)
        {
            return this.Compare(x as LPModel, y as LPModel);
        }

        /// <summary>
        /// Compares two LP models.
        /// </summary>
        /// <param name="first">The first model.</param>
        /// <param name="second">The second model.</param>
        /// <returns>Zero if equal, -1 or +1 based on model size, and 2 if model coefficients are different.</returns>
        public int Compare(LPModel first, LPModel second)
        {
            this.differences.Clear();

            if (ReferenceEquals(first, second))
            {
                return 0;
            }

            if (first == null)
            {
                return -1;
            }

            if (second == null)
            {
                return 1;
            }

            return this.CompareInternal(first, second);
        }

        /// <summary>
        /// Internal method to compare two LP models.
        /// </summary>
        /// <param name="first">The first LP model.</param>
        /// <param name="second">The second LP model.</param>
        /// <returns>The comparison result.</returns>
        private int CompareInternal(LPModel first, LPModel second)
        {
            this.firstObjective = first.Objective;
            this.secondObjective = second.Objective;

            this.CompareRowType(first.RowTypes, second.RowTypes);

            this.CompareAMatrix(first.A, second.A);

            this.CompareVector(
                first.A[first.Objective],
                second.A[second.Objective],
                $"Objective {first.Objective}/{second.Objective}");

            this.CompareVector(
                first.B[first.SelectedRhsName],
                second.B[second.SelectedRhsName],
                $"RHS {first.SelectedRhsName}/{second.SelectedRhsName}");

            this.CompareVector(
                first.L[first.SelectedBoundName],
                second.L[second.SelectedBoundName],
                $"Lower bound {first.SelectedBoundName}/{second.SelectedBoundName}");

            this.CompareVector(
                first.U[first.SelectedBoundName],
                second.U[second.SelectedBoundName],
                $"Upper bound {first.SelectedBoundName}/{second.SelectedBoundName}");

            this.CompareVector(
                first.R[first.SelectedRangeName],
                second.R[second.SelectedRangeName],
                $"Range {first.SelectedRangeName}/{second.SelectedRangeName}");

            return this.differences.Count;
        }

        /// <summary>
        /// Compares two vectors of row types.
        /// </summary>
        /// <param name="first">The first row type vector.</param>
        /// <param name="second">The second row type vector.</param>
        private void CompareRowType(SparseVector<string, MpsRow> first, SparseVector<string, MpsRow> second)
        {
            if (ReferenceEquals(first, second))
            {
                return;
            }

            if (first == null || second == null)
            {
                this.differences.Add(string.Format("Row type vector null {0}", first == null ? "first" : "second"));
                return;
            }

            if (first.Count != second.Count)
            {
                this.differences.Add($"Row type vector size {first.Count} != {second.Count}");
            }

            foreach (var rowIndex in first.Indices.Union(second.Indices))
            {
                if (rowIndex == this.firstObjective || rowIndex == this.secondObjective)
                {
                    continue;
                }

                var x = first[rowIndex];
                var y = second[rowIndex];
                if (x != y)
                {
                    this.differences.Add($"RowType[{rowIndex}] {x} != {y}");
                }
            }

            if (first[this.firstObjective] != second[this.secondObjective])
            {
                this.differences.Add(string.Format(
                    "Objective rowType unequal {0} != {1}",
                    first[this.firstObjective],
                    second[this.secondObjective]));
            }
        }

        /// <summary>
        /// Compares the A matrix of two LP models.
        /// </summary>
        /// <param name="first">The first A matrix.</param>
        /// <param name="second">The second A matrix.</param>
        private void CompareAMatrix(SparseMatrix<string, double> first, SparseMatrix<string, double> second)
        {
            if (ReferenceEquals(first, second))
            {
                return;
            }

            if (first == null || second == null)
            {
                this.differences.Add(string.Format("A matrix null {0}", first == null ? "first" : "second"));
                return;
            }

            var firstShape = first.Shape;
            var secondShape = second.Shape;
            if (firstShape.Item1 != secondShape.Item1 || firstShape.Item2 != secondShape.Item2)
            {
                this.differences.Add($"A matrix shape {first} != {second}");
            }

            var firstRows = new HashSet<string>(first.RowIndices);
            firstRows.Remove(this.firstObjective);

            var secondRows = new HashSet<string>(second.RowIndices);
            secondRows.Remove(this.secondObjective);

            if (!firstRows.SetEquals(secondRows))
            {
                foreach (var rowIndex in firstRows.Except(secondRows))
                {
                    this.differences.Add($"A[{rowIndex}] row vector in first but not second");
                }

                foreach (var rowIndex in secondRows.Except(firstRows))
                {
                    this.differences.Add($"A[{rowIndex}] row vector in second but not first");
                }
            }

            firstRows.IntersectWith(secondRows);
            var commonRows = firstRows;
            foreach (var rowIndex in commonRows)
            {
                this.CompareVector(first[rowIndex], second[rowIndex], $"A[{rowIndex}] row vector");
            }
        }

        /// <summary>
        /// Compares two double vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <param name="tag">The tag to use in difference messages.</param>
        private void CompareVector(
            SparseVector<string, double> first,
            SparseVector<string, double> second,
            string tag)
        {
            if (ReferenceEquals(first, second))
            {
                return;
            }

            if (first == null || second == null)
            {
                this.differences.Add(string.Format("{0} null {1}", tag, first == null ? "first" : "second"));
                return;
            }

            foreach (var index in first.Indices.Union(second.Indices))
            {
                var x = first[index];
                var y = second[index];
                if (Math.Abs(x - y) > this.Tolerance)
                {
                    this.differences.Add($"{tag} element {index} {x} != {y}");
                }
            }
        }
    }
}
