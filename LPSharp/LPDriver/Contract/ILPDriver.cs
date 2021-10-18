// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILPDriver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    using System.Collections.Generic;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Interface for the LP driver.
    /// </summary>
    public interface ILPDriver
    {
        /// <summary>
        /// Gets the enumeration of models.
        /// </summary>
        IReadOnlyDictionary<string, LPModel> Models { get; }

        /// <summary>
        /// Gets the enumeration of solvers.
        /// </summary>
        IReadOnlyDictionary<string, ILPInterface> Solvers { get; }

        /// <summary>
        /// Gets the MPS file reader.
        /// </summary>
        MpsReader MpsReader { get; }

        /// <summary>
        /// Gets or sets the default solver key.
        /// </summary>
        string DefaultSolverKey { get; set; }

        /// <summary>
        /// Clears stored models and solvers.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds or updates a model.
        /// </summary>
        /// <param name="key">The model key.</param>
        /// <param name="solver">The model.</param>
        /// <returns>True if added successfully.</returns>
        bool AddModel(string key, LPModel solver);

        /// <summary>
        /// Gets a model by key.
        /// </summary>
        /// <param name="key">The model key.</param>
        /// <returns>The model.</returns>
        LPModel GetModel(string key);

        /// <summary>
        /// Adds or updates a solver.
        /// </summary>
        /// <param name="key">The solver key.</param>
        /// <param name="solverType">The solver type.</param>
        /// <returns>True if added successfully.</returns>
        bool CreateSolver(string key, SolverType solverType);

        /// <summary>
        /// Gets a solver by key.
        /// </summary>
        /// <param name="key">The solver key.</param>
        /// <returns>The solver.</returns>
        ILPInterface GetSolver(string key);
    }
}
