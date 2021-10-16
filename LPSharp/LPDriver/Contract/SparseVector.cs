// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseVector.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
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
        /// Map of index.
        /// </summary>
        private readonly Dictionary<Tindex, Tvalue> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseVector{I, V}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public SparseVector(int capacity = 0)
        {
            this.store = new Dictionary<Tindex, Tvalue>(capacity);
        }

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
        /// Clears the vector.
        /// </summary>
        public void Clear()
        {
            this.store.Clear();
        }

        /// <summary>
        /// Gets a dense array of values.
        /// </summary>
        /// <param name="indices">The dictionary mapping vector index to an array index.</param>
        /// <returns>The array of values.</returns>
        public Tvalue[] ToArray(out Dictionary<Tindex, int> indices)
        {
            indices = new Dictionary<Tindex, int>(this.store.Count);
            var values = new Tvalue[this.store.Count];
            int i = 0;
            foreach (var kv in this.store)
            {
                indices[kv.Key] = i;
                values[i] = kv.Value;
                i++;
            }

            return values;
        }
    }
}
