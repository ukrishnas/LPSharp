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
        /// Sets solver parameters.
        /// </summary>
        /// <param name="solverParameters">The solver parameters.</param>
        void SetParameters(SolverParameters solverParameters);

        /// <summary>
        /// Clears the linear program and the stored state from the previous Solve() call. This
        /// is a combination of Reset() and clearing the stored model. Previously set parameters
        /// are not cleared.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Resets the stored state from the previous Solve() call. After reset, a new call to Solve()
        /// will take exactly the same time as before. The stored model and parameters are not affected.
        /// </summary>
        public void Reset();

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
    }
}
