// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseMatrix.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Model
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
            this.ColumnDefault = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix{Tindex, Tvalue}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public SparseMatrix(Tvalue defaultValue)
            : base()
        {
            this.columnIndices = new HashSet<Tindex>();
            this.ColumnDefault = defaultValue;
        }

        /// <summary>
        /// Gets or sets the default value for each column in the row vector.
        /// </summary>
        public Tvalue ColumnDefault { get; set; }

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
        /// Gets the number of column indices. This count is the number of indices across all rows.
        /// If a matrix is ((1, 0), (0, 1)), column count is one because each row has only one
        /// element, but column index count is two because there are two indices.
        /// </summary>
        public int ColumnIndexCount => this.columnIndices.Count;

        /// <summary>
        /// Gets the rectangular shape of the matrix as a tuple of row and column index count.
        /// </summary>
        public Tuple<int, int> RectangularShape => new(this.RowCount, this.ColumnIndexCount);

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
                    return this.ColumnDefault;
                }

                return this[rowIndex][colIndex];
            }

            set
            {
                if (rowIndex != null)
                {
                    if (this[rowIndex] == null)
                    {
                        this[rowIndex] = new SparseVector<Tindex, Tvalue>(this.ColumnDefault);
                    }

                    if (colIndex != null)
                    {
                        this[rowIndex][colIndex] = value;
                        this.columnIndices.Add(colIndex);
                    }
                }
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
            if (success)
            {
                // Remove row if it is empty.
                if (this[rowIndex].Count == 0)
                {
                    base.Remove(rowIndex);
                }

                // Remove column index it is not used.
                bool inUse = false;
                foreach (var row in this.Elements)
                {
                    inUse |= row.Has(colIndex);
                    if (inUse)
                    {
                        break;
                    }
                }

                if (!inUse)
                {
                    this.columnIndices.Remove(colIndex);
                }
            }

            return true;
        }
    }
}
