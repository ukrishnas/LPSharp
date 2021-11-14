// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPModelComparer.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Collections;
    using System.Collections.Generic;

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
        /// Initializes a new instance of the <see cref="LPModelComparer"/> class.
        /// </summary>
        public LPModelComparer()
        {
            this.differences = new List<string>();
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

            return 2;
        }
    }
}
