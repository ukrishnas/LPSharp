// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetModels.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Collections.Generic;
    using System.Management.Automation;

    /// <summary>
    /// Gets the stored models.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Models")]
    public class GetModels : LPCmdlet
    {
        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.LPDriver.Models.Count == 0)
            {
                this.WriteHost("No models present. Please add a model using Read-Mps");
                return;
            }

            var models = new Dictionary<string, string>();
            foreach (var kv in this.LPDriver.Models)
            {
                models.Add(kv.Key, kv.Value.ToString());
            }

            this.WriteObject(models);
        }
    }
}