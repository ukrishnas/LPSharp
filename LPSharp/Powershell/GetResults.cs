// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetResults.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;

    /// <summary>
    /// Gets the execution results.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Results")]
    public class GetResults : LPCmdlet
    {
        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.LPDriver.Results.Count == 0)
            {
                this.WriteHost("No execution results present");
                return;
            }

            this.WriteObject(this.LPDriver.Results);
        }
    }
}