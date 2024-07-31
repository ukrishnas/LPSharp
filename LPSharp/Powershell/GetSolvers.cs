// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSolvers.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.Powershell
{
    using System.Collections.Generic;
    using System.Management.Automation;
    using LPSharp.LPDriver.Model;

    /// <summary>
    /// Gets the created solvers.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Solvers")]
    public class GetSolvers : LPCmdlet
    {
        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.LPDriver.Solvers.Count == 0)
            {
                this.WriteHost("No solvers present. Please add a solver using Set-Solver");
                return;
            }

            var solvers = new Dictionary<string, string>();
            foreach (var kv in this.LPDriver.Solvers)
            {
                if (kv.Value is LPSolverAbstract solver)
                {
                    solvers.Add(kv.Key, solver.ToString());
                }
            }

            this.WriteObject(solvers);
        }
    }
}