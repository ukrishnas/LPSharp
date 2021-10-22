// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClpSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    /// <summary>
    /// Represents the interface implementation for CLP solver.
    /// </summary>
    public class ClpSolver : OrtoolsSolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClpSolver"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public ClpSolver(string key)
            : base(key, "CLP_LINEAR_PROGRAMMING")
        {
        }
    }
}