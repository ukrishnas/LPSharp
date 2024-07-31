﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompareModels.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.Powershell
{
    using System.Linq;
    using System.Management.Automation;
    using LPSharp.LPDriver.Model;

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

        /// <summary>
        /// Gets or sets the tolerance for double precision comparison.
        /// </summary>
        [Parameter]
        public double? Tolerance { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            var first = this.LPDriver.GetModel(this.First);
            if (first == null)
            {
                this.WriteHost($"Model {this.First} not found. Please enter model keys, not path names.");
                return;
            }

            var second = this.LPDriver.GetModel(this.Second);
            if (second == null)
            {
                this.WriteHost($"Model {this.Second} not found. Please enter model keys, not path names.");
                return;
            }

            var comparer = new LPModelComparer();
            if (this.Tolerance.HasValue)
            {
                comparer.Tolerance = this.Tolerance.Value;
            }

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
