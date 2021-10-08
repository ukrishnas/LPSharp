// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetMps.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;

    /// <summary>
    /// Gets information from the MPS reader.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Mps")]
    public class GetMps : LPCmdlet
    {
        /// <summary>
        /// Gets or sets a value indicating whether to get errors from the last read.
        /// </summary>
        [Parameter]
        public SwitchParameter Errors { get; set; }

        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            // Return errors if no parameter is specified.
            if (!this.Errors)
            {
                this.Errors = true;
            }

            var reader = this.LPDriver.MpsReader;

            if (this.Errors)
            {
                if (reader.Errors.Count == 0)
                {
                    this.WriteHost("No read errors");
                    return;
                }

                this.WriteObject(this.LPDriver.MpsReader.Errors);
                return;
            }
        }
    }
}