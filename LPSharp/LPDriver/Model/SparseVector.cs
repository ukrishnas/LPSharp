// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseVector.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a sparse vector.
    /// </summary>
    /// <typeparam name="Tindex">The type of index.</typeparam>
    /// <typeparam name="Tvalue">The type of value.</typeparam>
    public class SparseVector<Tindex, Tvalue> : IEquatable<SparseVector<Tindex, Tvalue>>
    {
        /// <summary>
        /// Stores vector elements as a dictionary mapping element index to value.
        /// </summary>
        private readonly Dictionary<Tindex, Tvalue> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseVector{Tindex, Tvalue}"/> class.
        /// </summary>
        public SparseVector()
        {
            this.store = new Dictionary<Tindex, Tvalue>();
            this.Default = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseVector{Tindex, Tvalue}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public SparseVector(Tvalue defaultValue)
        {
            this.store = new Dictionary<Tindex, Tvalue>();
            this.Default = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseVector{Tindex, Tvalue}"/> class.
        /// </summary>
        /// <param name="dict">The enumeration of index-element pairs.</param>
        /// <param name="defaultValue">The default value.</param>
        public SparseVector(IEnumerable<KeyValuePair<Tindex, Tvalue>> dict, Tvalue defaultValue = default)
            : this(defaultValue)
        {
            foreach (var kv in dict)
            {
                this[kv.Key] = kv.Value;
            }
        }

        /// <summary>
        /// Gets or sets the default value to return if an index is not present.
        /// </summary>
        public Tvalue Default { get; set; }

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
                if (index != null && this.store.TryGetValue(index, out Tvalue value))
                {
                    return value;
                }
                else
                {
                    return this.Default;
                }
            }

            set
            {
                if (index != null)
                {
                    this.store[index] = value;
                }
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;

            hash = (hash * 23) + this.Default.GetHashCode();
            foreach (var kv in this.store)
            {
                hash = (hash * 23) + kv.Key.GetHashCode();
                hash = (hash * 23) + kv.Value.GetHashCode();
            }

            return hash;
        }

        /// <inheritdoc />
        public override bool Equals(object other)
        {
            return this.Equals(other as SparseVector<Tindex, Tvalue>);
        }

        /// <inheritdoc />
        public bool Equals(SparseVector<Tindex, Tvalue> other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.Count != other.Count ||
                !Equals(this.Default, other.Default))
            {
                return false;
            }

            foreach (var kv in this.store)
            {
                if (!Equals(other[kv.Key], kv.Value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the vector has the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>True if element is present.</returns>
        public bool Has(Tindex index)
        {
            return index != null && this.store.ContainsKey(index);
        }

        /// <summary>
        /// Removes an element.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>True if removed successfully, false otherwise.</returns>
        public bool Remove(Tindex index)
        {
            if (!this.Has(index))
            {
                return false;
            }

            this.store.Remove(index);
            return true;
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

        /// <summary>
        /// Creates a clone of this object that is a shallow copy. Elements that are reference
        /// objects will not be cloned.
        /// </summary>
        /// <returns>A clone of this object.</returns>
        public SparseVector<Tindex, Tvalue> Clone()
        {
            var clone = new SparseVector<Tindex, Tvalue>(this.Default);

            foreach (var kv in this.store)
            {
                clone.store[kv.Key] = kv.Value;
            }

            return clone;
        }
    }
}
