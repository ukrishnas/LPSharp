// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSolvers.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Collections.Generic;
    using System.Management.Automation;

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
                solvers.Add(kv.Key, kv.Value.ToString());
            }

            this.WriteObject(solvers);
        }
    }
}