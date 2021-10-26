// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILPInterface.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Represents the interface to an LP solver.
    /// </summary>
    public interface ILPInterface
    {
        /// <summary>
        /// Loads a model.
        /// </summary>
        /// <param name="model">The solver model.</param>
        /// <returns>True if the model was loaded successfully, false otherwise.</returns>
        bool Load(LPModel model);

        /// <summary>
        /// Solves the model.
        /// </summary>
        /// <returns>True if the result status is optimal, false otherwise.</returns>
        bool Solve();

        /// <summary>
        /// Sets a solver parameter.
        /// </summary>
        /// <param name="parameter">The parameter type.</param>
        /// <param name="arguments">The parameter arguments.</param>
        void Set(SolverParameter parameter, params object[] arguments);
    }
}
