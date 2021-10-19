// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
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
            var solver = this.LPDriver.GetSolver(solverKey);
            if (solver == null)
            {
                this.WriteHost($"Solver {solverKey} not found. Please verify key or create solver with Set-Solver");
                return;
            }

            if (solver is not LPSolverAbstract solverAbstract)
            {
                this.WriteHost("Unable to access solver abstract");
                return;
            }

            this.WriteHost($"Loading model {this.ModelKey}");
            if (!solver.Load(model))
            {
                this.WriteHost($"Unable to load model {this.ModelKey}, possibly invalid model");
                return;
            }

            this.WriteHost($"Solving model {this.ModelKey}...");
            var isoptimal = solver.Solve();
            this.WriteHost("Solved model {0} result={1}", this.ModelKey, isoptimal ? "optimal" : "not optimal");

            var resultKey = $"{solverAbstract.Key}_{this.ModelKey}";
            this.LPDriver.AddResult(resultKey, solverAbstract.Metrics);
        }
    }
}
