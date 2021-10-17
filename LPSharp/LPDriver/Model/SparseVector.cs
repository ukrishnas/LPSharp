// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseVector.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a sparse vector.
    /// </summary>
    /// <typeparam name="Tindex">The type of index.</typeparam>
    /// <typeparam name="Tvalue">The type of value.</typeparam>
    public class SparseVector<Tindex, Tvalue>
    {
        /// <summary>
        /// Stores vector elements as a dictionary mapping element index to value.
        /// </summary>
        private readonly Dictionary<Tindex, Tvalue> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseVector{Tindex, Tvalue}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public SparseVector(int capacity = 0)
        {
            this.store = new Dictionary<Tindex, Tvalue>(capacity);
        }

        /// <summary>
        /// Gets the number of elements in the vector.
        /// </summary>
        public int Count => this.store.Count;

        /// <summary>
        /// Gets the enumeration of vector indices.
        /// </summary>
        public IEnumerable<Tindex> Indices => this.store.Keys;

        /// <summary>
        /// Gets the enumeration of vector elements.
        /// </summary>
        public IEnumerable<Tvalue> Elements => this.store.Values;

        /// <summary>
        /// Gets or sets an element of the vector.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value.</returns>
        public Tvalue this[Tindex index]
        {
            get
            {
                if (this.store.TryGetValue(index, out Tvalue value))
                {
                    return value;
                }
                else
                {
                    return default;
                }
            }

            set
            {
                this.store[index] = value;
            }
        }

        /// <summary>
        /// Returns true if the vector has the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>True if index is present.</returns>
        public bool Has(Tindex index)
        {
            return this.store.ContainsKey(index);
        }

        /// <summary>
        /// Clears the vector.
        /// </summary>
        public void Clear()
        {
            this.store.Clear();
        }

        /// <summary>
        /// Gets dense arrays of indices and values. Vector elements and their indices can be directly
        /// accessed in constant time. The order of the elements in the arrays is the insertion order.
        /// </summary>
        /// <param name="indices">The array of indices.</param>
        /// <returns>The array of values.</returns>
        public Tvalue[] ToArray(out Tindex[] indices)
        {
            indices = new Tindex[this.store.Count];
            var values = new Tvalue[this.store.Count];
            int i = 0;
            foreach (var kv in this.store)
            {
                indices[i] = kv.Key;
                values[i] = kv.Value;
                i++;
            }

            return values;
        }
    }
}
