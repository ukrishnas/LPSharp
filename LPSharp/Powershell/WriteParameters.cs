// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WriteParameters.cs">
// Copyright (c) Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.IO;
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Writes solver parameters from file.
    /// </summary>
    [Cmdlet(VerbsCommunications.Write, "Parameters")]
    public class WriteParameters : LPCmdlet
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
            var solverParameters = this.LPDriver.SolverParameters;
            if (this.LPDriver.SolverParameters == null)
            {
                this.WriteHost("Null solver parameters");
                return;
            }

            using (var stream = File.Open(this.FileName, FileMode.Create))
            {
                Utility.TrySerialize(stream, solverParameters);
            }

            this.WriteHost($"Wrote solver parameters to {this.FileName}");
        }
    }
}
