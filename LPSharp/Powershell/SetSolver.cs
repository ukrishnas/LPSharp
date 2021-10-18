// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetSolver.cs" company="Microsoft Corporation">
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
    [Cmdlet(VerbsCommon.Set, "Solver")]
    public class SetSolver : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the solver key.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make this solver the default
        /// for all solver operations.
        /// </summary>
        [Parameter]
        public SwitchParameter Default { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to extend variable and constraint
        /// bounds to minus infinity.
        /// </summary>
        [Parameter]
        public SwitchParameter MinusInfinity { get; set; }

        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            var solver = this.LPDriver.GetSolver(this.Key);
            if (solver == null)
            {
                this.WriteHost($"Could not find solver key={this.Key}");
                return;
            }

            if (this.Default)
            {
                this.LPDriver.DefaultSolverKey = this.Key;
            }

            if (solver is not LPSolverAbstract solverAbstract)
            {
                this.WriteHost($"Unable to access bounds properties of solver key={this.Key}");
                return;
            }

            if (this.MinusInfinity)
            {
                solverAbstract.DefaultVariableBounds = new(double.NegativeInfinity, double.PositiveInfinity);
                solverAbstract.DefaultConstraintBounds = new(double.NegativeInfinity, double.PositiveInfinity);
            }

            this.WriteHost($"Solver key={this.LPDriver.DefaultSolverKey} is default solver");
            this.WriteHost(
                "Solver key={0}, default variable bounds=({1}, {2})",
                solverAbstract.Key,
                solverAbstract.DefaultVariableBounds.Item1,
                solverAbstract.DefaultVariableBounds.Item2);
            this.WriteHost(
                "Solver key={0}, default constraint bounds=({1}, {2})",
                solverAbstract.Key,
                solverAbstract.DefaultConstraintBounds.Item1,
                solverAbstract.DefaultConstraintBounds.Item2);
        }
    }
}
