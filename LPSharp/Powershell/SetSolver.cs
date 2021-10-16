// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Invokes the solver.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "Solver")]
    public class SetSolver : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the solver type.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public SolverType SolverType { get; set; }

        /// <summary>
        /// Gets or sets the key to store the solver.
        /// </summary>
        [Parameter]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make this solver the default
        /// for all solver operations.
        /// </summary>
        [Parameter]
        public SwitchParameter Default { get; set; }

        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            var key = this.Key ?? this.SolverType.ToString();

            if (!this.LPDriver.CreateSolver(key, this.SolverType, this.Default))
            {
                this.WriteHost($"Could not create solver type={this.SolverType}");
                return;
            }

            var defaultSolver = this.LPDriver.DefaultSolverKey ?? "(none)";
            this.WriteHost($"Set solver type={this.SolverType} key={key} default={defaultSolver}");
        }
    }
}
