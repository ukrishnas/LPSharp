// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlopSolver.cs">
// Copyright (c) Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    /// <summary>
    /// Represents the interface implementation for GLOP solver.
    /// </summary>
    public class GlopSolver : OrtoolsSolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlopSolver"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public GlopSolver(string key)
            : base(key, "GLOP_LINEAR_PROGRAMMING")
        {
        }
    }
}
