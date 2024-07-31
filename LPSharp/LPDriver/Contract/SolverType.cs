// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverType.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents the solver type.
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        /// Computational infrastructure for Operations Research (COIN-OR) linear programming solver.
        /// </summary>
        CLP,

        /// <summary>
        /// Google linear programming solver.
        /// </summary>
        GLOP,
    }
}
