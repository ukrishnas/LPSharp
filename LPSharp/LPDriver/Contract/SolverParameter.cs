// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolverParameter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents a solver parameter.
    /// </summary>
    public enum SolverParameter
    {
        /// <summary>
        /// Dual simplex method and options.
        /// </summary>
        DualSimplex,

        /// <summary>
        /// Primal simplex method and options.
        /// </summary>
        PrimalSimplex,

        /// <summary>
        /// Barrier method and options.
        /// </summary>
        BarrierMethod,
    }
}
