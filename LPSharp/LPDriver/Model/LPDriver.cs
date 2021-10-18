// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPDriver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Collections.Generic;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents the driver for LPSharp. It holds common state and methods.
    /// </summary>
    public class LPDriver : ILPDriver
    {
        /// <summary>
        /// The collection of models.
        /// </summary>
        private readonly Dictionary<string, LPModel> models;

        /// <summary>
        /// The collection of solvers.
        /// </summary>
        private readonly Dictionary<string, ILPInterface> solvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPDriver"/> class.
        /// </summary>
        public LPDriver()
        {
            this.models = new Dictionary<string, LPModel>();
            this.solvers = new Dictionary<string, ILPInterface>();
            this.MpsReader = new MpsReader();
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, LPModel> Models => this.models;

        /// <inheritdoc />
        public IReadOnlyDictionary<string, ILPInterface> Solvers => this.solvers;

        /// <inheritdoc />
        public MpsReader MpsReader { get; }

        /// <inheritdoc />
        public string DefaultSolverKey { get; set; }

        /// <inheritdoc />
        public void Clear()
        {
            this.models.Clear();
            this.solvers.Clear();
        }

        /// <inheritdoc />
        public bool AddModel(string key, LPModel solver)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            // Previous model is silently overwritten.
            this.models[key] = solver;
            return true;
        }

        /// <inheritdoc />
        public LPModel GetModel(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            this.models.TryGetValue(key, out LPModel model);
            return model;
        }

        /// <inheritdoc />
        public bool CreateSolver(string key, SolverType solverType)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            ILPInterface solver = null;
            switch (solverType)
            {
                case SolverType.GLOP:
                    solver = new GlopSolver(key);
                    break;

                default:
                    break;
            }

            if (solver == null)
            {
                return false;
            }

            // Previous model is silently overwritten.
            this.solvers[key] = solver;

            return true;
        }

        /// <inheritdoc />
        public ILPInterface GetSolver(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            this.solvers.TryGetValue(key, out ILPInterface solver);
            return solver;
        }
    }
}