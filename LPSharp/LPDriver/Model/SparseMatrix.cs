﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseMatrix.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a sparse matrix organized as a vector of row vectors.
    /// </summary>
    /// <typeparam name="Tindex">The type of index.</typeparam>
    /// <typeparam name="Tvalue">The type of value.</typeparam>
    public class SparseMatrix<Tindex, Tvalue> : SparseVector<Tindex, SparseVector<Tindex, Tvalue>>
    {
        /// <summary>
        /// The collection of column indices.
        /// </summary>
        private readonly HashSet<Tindex> columnIndices;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix{Tindex, Tvalue}"/> class.
        /// </summary>
        public SparseMatrix()
            : base()
        {
            this.columnIndices = new HashSet<Tindex>();
        }

        /// <summary>
        /// Gets the number of elements in the matrix.
        /// </summary>
        public new int Count
        {
            get => base.Count == 0 ? 0 : this.Elements.Sum(x => x.Count);
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int RowCount => base.Count;

        /// <summary>
        /// Gets the number of columns. Note that for jagged matrices, it
        /// returns the maximum column count.
        /// </summary>
        public int ColumnCount => base.Count == 0 ? 0 : this.Elements.Max(x => x.Count);

        /// <summary>
        /// Gets the shape of the matrix as a tuple of row and column counts.
        /// </summary>
        public Tuple<int, int> Shape
        {
            get => new(this.RowCount, this.ColumnCount);
        }

        /// <summary>
        /// Gets the row indices.
        /// </summary>
        public IEnumerable<Tindex> RowIndices => this.Indices;

        /// <summary>
        /// Gets the column indices.
        /// </summary>
        public IEnumerable<Tindex> ColumnIndices => this.columnIndices;

        /// <summary>
        /// Gets or sets an element of the vector.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</permission>
        /// <returns>The value.</returns>
        public Tvalue this[Tindex rowIndex, Tindex colIndex]
        {
            get
            {
                if (this[rowIndex] == null)
                {
                    return default;
                }

                return this[rowIndex][colIndex];
            }

            set
            {
                if (this[rowIndex] == null)
                {
                    this[rowIndex] = new SparseVector<Tindex, Tvalue>();
                }

                this[rowIndex][colIndex] = value;
                this.columnIndices.Add(colIndex);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var shape = this.Shape;
            return $"({shape.Item1}, {shape.Item2})";
        }

        /// <summary>
        /// Returns true if the vector has the specified index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if element is present.</returns>
        public bool Has(Tindex rowIndex, Tindex colIndex)
        {
            return this.Has(rowIndex) && this[rowIndex].Has(colIndex);
        }

        /// <summary>
        /// Removes the element from the matrix.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if element was removed, false otherwise.</returns>
        public bool Remove(Tindex rowIndex, Tindex colIndex)
        {
            if (!this.Has(rowIndex))
            {
                return false;
            }

            var success = this[rowIndex].Remove(colIndex);
            if (success && this[rowIndex].Count == 0)
            {
                base.Remove(rowIndex);
            }

            return true;
        }
    }
}
