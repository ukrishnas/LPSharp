// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Contract;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Invokes the solver.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "Solver")]
    public class SetSolver : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the solver key.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the solver type to create.
        /// </summary>
        [Parameter]
        [Alias("Create")]
        public SolverType? CreateSolverType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make this solver the default
        /// for all solver operations.
        /// </summary>
        [Parameter]
        public SwitchParameter Default { get; set; }

        /// <summary>
        /// Gets or sets the solver parameters.
        /// </summary>
        [Parameter]
        public SwitchParameter Parameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clear the solver state. This resets
        /// the extracted model and clears the loaded model. This does not affect parameters.
        /// </summary>
        [Parameter]
        public SwitchParameter Clear { get; set; }

        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            ILPInterface solver;

            if (this.CreateSolverType != null)
            {
                var solverType = this.CreateSolverType.Value;
                solver = this.LPDriver.CreateSolver(this.Key, solverType);
                if (solver == null)
                {
                    this.WriteHost($"Could not create solver type={solverType}");
                    return;
                }

                this.WriteHost($"Solver {solver} created");
            }
            else
            {
                solver = this.LPDriver.GetSolver(this.Key);
                if (solver == null)
                {
                    this.WriteHost($"Solver {this.Key} key not found");
                    return;
                }
            }

            if (this.Default)
            {
                this.LPDriver.DefaultSolverKey = this.Key;
                this.WriteHost($"Solver {this.LPDriver.DefaultSolverKey} is default solver");
            }

            var solver2 = solver as LPSolverAbstract;

            if (this.Parameters)
            {
                solver.SetParameters(this.LPDriver.SolverParameters);
                this.WriteHost($"Solver {solver2.Key} parameters set with previously read parameters");
            }

            if (this.Clear)
            {
                solver.Clear();
                this.WriteHost($"Solver {solver2.Key} state and model cleared");
            }
        }
    }
}
