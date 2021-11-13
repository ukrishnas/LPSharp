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
        /// Clears the linear program and the stored state from the previous Solve() call. Parameters
        /// are not cleared.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Loads a model into the solver.
        /// </summary>
        /// <param name="model">The solver model.</param>
        /// <returns>True if the model was loaded successfully, false otherwise.</returns>
        bool Load(LPModel model);

        /// <summary>
        /// Sets solver parameters.
        /// </summary>
        /// <param name="solverParameters">The solver parameters.</param>
        void SetParameters(SolverParameters solverParameters);

        /// <summary>
        /// Solves the model.
        /// </summary>
        /// <returns>True if the result status is optimal, false otherwise.</returns>
        bool Solve();

        /// <summary>
        /// Writes the solver model to file.
        /// </summary>
        /// <param name="pathName">The output file path.</param>
        void Write(string pathName);
    }
}
