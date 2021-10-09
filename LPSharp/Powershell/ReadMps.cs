// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadMps.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Management.Automation;

    /// <summary>
    /// Reads a model from a file in MPS format.
    /// </summary>
    [Cmdlet(VerbsCommunications.Read, "Mps")]
    public class ReadMps : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the MPS file name.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the key to store the model.
        /// </summary>
        [Parameter]
        public string Key { get; set; }

        /// <summary>
        /// Process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (!File.Exists(this.FileName))
            {
                this.WriteHost($"MPS file {this.FileName} not found");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var model = this.LPDriver.MpsReader.Read(this.FileName);
            stopwatch.Stop();

            var key = this.Key ?? model.Name;
            if (string.IsNullOrEmpty(key))
            {
                var guid = Guid.NewGuid().ToString();
                key = $"model_{guid}";
            }    

            this.LPDriver.AddModel(key, model);

            this.WriteHost(
                "Read MPS file {0} as model {1} in {2} ms, read_errors={3}, model_key={4}",
                this.FileName,
                model,
                stopwatch.ElapsedMilliseconds,
                this.LPDriver.MpsReader.Errors.Count,
                key);
        }
    }
}
