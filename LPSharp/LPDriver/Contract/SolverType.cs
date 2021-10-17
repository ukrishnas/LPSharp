// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverType.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents the solver type.
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        /// Computational infrastruction for Operations Research (COIN-OR) linear programming solver.
        /// </summary>
        CLP,

        /// <summary>
        /// Google linear programming solver.
        /// </summary>
        GLOP,

        /// <summary>
        /// Microsoft Solver Foundation linear programming solver.
        /// </summary>
        MSFLP,
    }
}
