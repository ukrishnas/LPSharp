// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverImplementation.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents the name of the solver.
    /// </summary>
    public enum SolverImplementation
    {
        /// <summary>
        /// Microsoft Solver Foundation.
        /// </summary>
        MSF = 0,

        /// <summary>
        /// Computational Infrastructure for Operational Research LP Solver (COIN-OR LP).
        /// </summary>
        CLP,

        /// <summary>
        /// Google Operational Tools Linear Optimization Program.
        /// </summary>
        GLOP,
    }
}
