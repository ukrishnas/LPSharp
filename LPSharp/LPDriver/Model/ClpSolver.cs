// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClpSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    /// <summary>
    /// Represents the interface implementation for CLP solver. This is currently implemented
    /// as an OrTool solver and requires CLP to packaged with the OrTools library. Use this only
    /// if LPSharp is built with a Google.OrTools nuget package with a 3-number version string.
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