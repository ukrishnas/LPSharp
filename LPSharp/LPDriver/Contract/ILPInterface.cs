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
        /// Resets the linear program and the stored state from the previous Solve() call. After
        /// reset, a new call to Solve() will take exactly the same time as before.
        /// </summary>
        public void Reset();

        /// <summary>
        /// Loads a model.
        /// </summary>
        /// <param name="model">The solver model.</param>
        /// <returns>True if the model was loaded successfully, false otherwise.</returns>
        bool Load(LPModel model);

        /// <summary>
        /// Sets a solver parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="values">The parameter values.</param>
        void Set(SolverParameter name, params object[] values);

        /// <summary>
        /// Solves the model.
        /// </summary>
        /// <returns>True if the result status is optimal, false otherwise.</returns>
        bool Solve();
    }
}
