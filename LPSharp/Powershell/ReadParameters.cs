// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadParameters.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.IO;
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Contract;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Reads solver parameters from file.
    /// </summary>
    [Cmdlet(VerbsCommunications.Read, "Parameters")]
    public class ReadParameters : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string FileName { get; set; }

        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            var solverParameters = Utility.ReadSolverParameters(this.FileName);
            if (solverParameters == null)
            {
                this.WriteHost($"Solver parameters file {this.FileName} not found or invalid");
                return;
            }

            this.LPDriver.SolverParameters = solverParameters;
            this.WriteHost($"Read solver parameters from {this.FileName}");
        }
    }
}
