// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Creates a new solver.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Solver")]
    public class NewSolver : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the solver type.
        /// </summary>
        [Parameter(Mandatory = true)]
        [Alias("type")]
        public SolverType SolverType { get; set; }

        /// <summary>
        /// Gets or sets the solver key.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (!this.LPDriver.CreateSolver(this.Key, this.SolverType))
            {
                this.WriteHost($"Could not create solver type={this.SolverType}");
                return;
            }

            var solver = this.LPDriver.GetSolver(this.Key);
            this.WriteHost($"Created new solver {solver}");
        }
    }
}
