// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeSolver.cs">
// Copyright(c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Invokes the solver.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "Solver")]
    public class InvokeSolver : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the model key.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        [Alias("Model")]
        public string ModelKey { get; set; }

        /// <summary>
        /// Gets or sets the solver key.
        /// </summary>
        [Parameter]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the output folder.
        /// </summary>
        [Parameter]
        public string OutputFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to write the model extracted from the solver
        /// to an MPS file. The file has the same name as the model key with an MPS extension.
        /// </summary>
        [Parameter]
        public SwitchParameter WriteModel { get; set; }

        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            var model = this.LPDriver.GetModel(this.ModelKey);
            if (model == null)
            {
                this.WriteHost($"Model {this.ModelKey} not found. Please verify key or load model with Read-Mps");
                return;
            }

            var solverKey = this.Key ?? this.LPDriver.DefaultSolverKey;
            if (this.LPDriver.GetSolver(solverKey) is not LPSolverAbstract solver)
            {
                this.WriteHost($"Solver {solverKey} not found. Please verify key or create solver with Set-Solver");
                return;
            }

            var outputFolder = Utility.CreateOutputFolder(this.OutputFolder);
            var scenario = $"{solver.Key}_{model.Name}";

            var result = this.RunAndCollect(solver, model, scenario, outputFolder);

            if (result != null)
            {
                this.LPDriver.AddResult(scenario, result);
                foreach (var kv in result)
                {
                    this.WriteHost("{0,-20} {1,25}", kv.Key, kv.Value);
                }
            }
        }

        /// <summary>
        /// Runs the solver and collects the results.
        /// </summary>
        /// <param name="solver">The solver.</param>
        /// <param name="model">The model.</param>
        /// <param name="scenario">The scenario key.</param>
        /// <param name="folder">The output folder to place results.</param>
        /// <returns>The solver execution result.</returns>
        private ExecutionResult RunAndCollect(LPSolverAbstract solver, LPModel model, string scenario, string folder)
        {
            if (!solver.Load(model))
            {
                this.WriteHost($"Solver {solver.Key} could not load model {model.Name}");
                return null;
            }

            this.WriteHost($"Solver {solver.Key} solving model {model.Name}...");
            solver.Solve();
            this.WriteHost($"Solver {solver.Key} solved model {model.Name} result={solver.ResultStatus}");

            if (this.WriteModel)
            {
                var pathName = scenario.EndsWith("mps", StringComparison.OrdinalIgnoreCase)
                    ? Path.Combine(folder, scenario)
                    : Path.Combine(folder, $"{scenario}.mps");
                solver.WriteModel(pathName);
            }

            return solver.Metrics;
        }
    }
}
