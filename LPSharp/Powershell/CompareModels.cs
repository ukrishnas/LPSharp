// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompareModels.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Compares models.
    /// </summary>
    [Cmdlet(VerbsData.Compare, "Models")]
    public class CompareModels : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the first model key.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string First { get; set; }

        /// <summary>
        /// Gets or sets the second model key.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string Second { get; set; }

        /// <summary>
        /// Gets or sets the limit on the differences to return.
        /// </summary>
        [Parameter]
        public int? Limit { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            var first = this.LPDriver.GetModel(this.First);
            if (first == null)
            {
                this.WriteHost($"Model {this.First} not found");
            }

            var second = this.LPDriver.GetModel(this.Second);
            if (second == null)
            {
                this.WriteHost($"Model {this.Second} not found");
            }

            var comparer = new LPModelComparer();
            var result = comparer.Compare(first, second);
            var differences = comparer.Differences;
            this.WriteHost("Models {0} and {1} are {2}", first.Name, second.Name, result == 0 ? "equal" : "not equal");

            if (differences.Count > 0)
            {
                if (this.Limit == null || differences.Count <= this.Limit)
                {
                    this.WriteObject(differences);
                }
                else
                {
                    this.WriteObject(differences.Take(this.Limit.Value).ToList());

                }
            }
        }
    }
}
