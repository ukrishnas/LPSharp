// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearAll.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Management.Automation;

    /// <summary>
    /// Clears models and solvers.
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "All")]
    public class ClearAll : LPCmdlet
    {
        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.LPDriver.Clear();
        }
    }
}