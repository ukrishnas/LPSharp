// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearAll.cs">
// Copyright(c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
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