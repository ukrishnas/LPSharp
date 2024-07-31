// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILPInterface.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Contract
{
    using LPSharp.LPDriver.Model;

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
        /// Clears the linear program and the stored state from the previous Solve() call. Parameters
        /// are not cleared.
        /// </summary>
        void Clear();

        /// <summary>
        /// Loads a model into the solver.
        /// </summary>
        /// <param name="model">The solver model.</param>
        /// <returns>True if successfully loaded, false otherwise.</returns>
        bool Load(LPModel model);

        /// <summary>
        /// Solves the model.
        /// </summary>
        /// <returns>True if the result status is optimal, false otherwise.</returns>
        bool Solve();

        /// <summary>
        /// Writes the solver model to file.
        /// </summary>
        /// <param name="pathName">The output file path.</param>
        void WriteModel(string pathName);
    }
}
