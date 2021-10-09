// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearModels.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;

    /// <summary>
    /// Clears the stored models.
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "Models")]
    public class ClearModels : LPCmdlet
    {
        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.LPDriver.ClearModels();
        }
    }
}