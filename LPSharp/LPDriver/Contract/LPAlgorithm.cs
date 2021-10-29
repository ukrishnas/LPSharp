// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPAlgorithm.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents the LP algorithm.
    /// </summary>
    public enum LPAlgorithm
    {
        /// <summary>
        /// Solver default.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Dual simplex.
        /// </summary>
        Dual,

        /// <summary>
        /// Primal simplex.
        /// </summary>
        Primal,

        /// <summary>
        /// Barrier (interior-point) method.
        /// </summary>
        Barrier,
    }
}
