// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadMps.cs">
// Copyright(c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Diagnostics;
    using System.IO;
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Contract;

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
        /// Gets or sets the MPS file format.
        /// </summary>
        [Parameter]
        public MpsFormat? Format { get; set; }

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

            var mpsFormat = this.Format ?? MpsFormat.Fixed;

            var stopwatch = Stopwatch.StartNew();
            var model = this.LPDriver.MpsReader.Read(this.FileName, mpsFormat);
            stopwatch.Stop();

            var key = this.Key;
            if (string.IsNullOrEmpty(key))
            {
                key = Path.GetFileNameWithoutExtension(this.FileName);
            }

            // Change the model name to the key, since model names in files are usually generic values.
            model.Name = key;
            this.LPDriver.AddModel(key, model);

            this.WriteHost(
                "Read MPS file {0} as model {1} in {2} ms, read_errors={3}, model_key={4}",
                this.FileName,
                model,
                stopwatch.ElapsedMilliseconds,
                this.LPDriver.MpsReader.Errors.Count,
                key);

            if (this.LPDriver.MpsReader.Errors.Count > 0)
            {
                this.WriteHost($"Model {key} has errors");
                return;
            }

            // Return the model key.
            this.WriteObject(model.Name);
        }
    }
}
