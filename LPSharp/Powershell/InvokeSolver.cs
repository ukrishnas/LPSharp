// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Diagnostics;
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

            var solverAbstract = solver as LPSolverAbstract;

            var stopwatch = Stopwatch.StartNew();
            var loadResult = solver.Load(model);
            stopwatch.Stop();

            this.WriteHost(
                "Solver={0} model={1} loadResult={2} loadTime={3} ms",
                solverAbstract.Key,
                this.ModelKey,
                loadResult ? "success" : "failure (possibly invalid)",
                stopwatch.ElapsedMilliseconds);
            if (!loadResult)
            {
                this.WriteHost("Model considered invalid by LPModel");
                return;
            }

            stopwatch.Restart();
            solver.Solve();
            stopwatch.Stop();

            this.WriteHost(
                "Solver={0} model={1} solveTime={2} ms",
                solverAbstract.Key,
                this.ModelKey,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
