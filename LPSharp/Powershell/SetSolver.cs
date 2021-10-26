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
        /// Gets or sets the solver parameter name.
        /// </summary>
        [Parameter]
        [Alias("name")]
        public SolverParameter? SolverParameterName { get; set; }

        /// <summary>
        /// Gets or sets the parameter values.
        /// </summary>
        [Parameter]
        [Alias("value", "values")]
        public object[] SolverParameterValues { get; set; }

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

                this.WriteHost($"Created solver {solver}");
            }
            else
            {
                solver = this.LPDriver.GetSolver(this.Key);
                if (solver == null)
                {
                    this.WriteHost($"Could not find solver key={this.Key}");
                    return;
                }
            }

            if (this.Default)
            {
                this.LPDriver.DefaultSolverKey = this.Key;
                this.WriteHost($"Solver key={this.LPDriver.DefaultSolverKey} is default solver");
            }

            var solverAbstract = solver as LPSolverAbstract;

            if (this.SolverParameterName != null)
            {
                var name = this.SolverParameterName.Value;
                this.WriteHost(
                    "Set solver parameter solve key={0} name={1} values={2}",
                    solverAbstract.Key,
                    name,
                    this.SolverParameterValues?.Length);
                solver.Set(name, this.SolverParameterValues);
            }
        }
    }
}
