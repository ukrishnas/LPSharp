// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseMatrix.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a sparse matrix organized as a vector of row vectors.
    /// </summary>
    /// <typeparam name="Tindex">The type of index.</typeparam>
    /// <typeparam name="Tvalue">The type of value.</typeparam>
    public class SparseMatrix<Tindex, Tvalue> : SparseVector<Tindex, SparseVector<Tindex, Tvalue>>
    {
        /// <summary>
        /// The capacity of row or column vectors..
        /// </summary>
        private readonly int capacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseMatrix{Tindex, Tvalue}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity of a row or column vectors.</param>
        public SparseMatrix(int capacity = 0)
            : base(capacity)
        {
            this.capacity = capacity;
        }

        /// <summary>
        /// Gets the number of elements in the matrix.
        /// </summary>
        public new int Count
        {
            get => base.Count == 0 ? 0 : this.Elements.Sum(x => x.Count);
        }

        /// <summary>
        /// Gets the shape of the matrix as a tuple of number of row vectors, maximum count of column elements.
        /// </summary>
        public Tuple<int, int> Shape
        {
            get => new(base.Count, base.Count == 0 ? 0 : this.Elements.Max(x => x.Count));
        }

        /// <summary>
        /// Gets or sets an element of the vector.
        /// </summary>
        /// <param name="rowindex">The row index.</param>
        /// <param name="colindex">The column index.</permission>
        /// <returns>The value.</returns>
        public Tvalue this[Tindex rowindex, Tindex colindex]
        {
            get
            {
                if (this[rowindex] == null)
                {
                    return default;
                }

                return this[rowindex][colindex];
            }

            set
            {
                if (this[rowindex] == null)
                {
                    this[rowindex] = new SparseVector<Tindex, Tvalue>(this.capacity);
                }

                this[rowindex][colindex] = value;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var shape = this.Shape;
            return $"({shape.Item1}, {shape.Item2})";
        }
    }
}
