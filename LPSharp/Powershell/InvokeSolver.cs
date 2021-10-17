// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Diagnostics;
    using System.Management.Automation;

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
        [Alias("Solver")]
        public string SolverKey { get; set; }

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

            var solverKey = this.SolverKey ?? this.LPDriver.DefaultSolverKey;
            var solver = this.LPDriver.GetSolver(solverKey);
            if (solver == null)
            {
                this.WriteHost($"Solver {solverKey} not found. Please verify key or create solver with Set-Solver");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            solver.Load(model);
            stopwatch.Stop();
            this.WriteHost($"Loaded model {this.ModelKey} into {solver} in {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            solver.Solve();
            stopwatch.Stop();
            this.WriteHost($"Solved model {this.ModelKey} in {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
