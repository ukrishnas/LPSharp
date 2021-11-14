// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetModel.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.Powershell
{
    using System;
    using System.Management.Automation;

    /// <summary>
    /// Sets model properties.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "Model")]
    public class SetModel : LPCmdlet
    {
        /// <summary>
        /// Gets or sets the model key.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the bound name.
        /// </summary>
        [Parameter]
        [Alias("Bound")]
        public string BoundName { get; set; }

        /// <summary>
        /// Gets or sets the right hand side name.
        /// </summary>
        [Parameter]
        [Alias("Rhs")]
        public string RhsName { get; set; }

        /// <summary>
        /// Gets or sets the range name.
        /// </summary>
        [Parameter]
        [Alias("Range")]
        public string RangeName { get; set; }

        /// <summary>
        /// Gets or sets the default variable bounds.
        /// </summary>
        [Parameter]
        public string DefaultVariableBound { get; set; }

        /// <summary>
        /// Gets or sets the default constraint bounds.
        /// </summary>
        [Parameter]
        public string DefaultConstraintBound { get; set; }

        /// <summary>
        /// The process record.
        /// </summary>
        protected override void ProcessRecord()
        {
            var model = this.LPDriver.GetModel(this.Key);
            if (model == null)
            {
                this.WriteHost($"Model {this.Key} not found");
                return;
            }

            if (!string.IsNullOrEmpty(this.BoundName))
            {
                model.SelectedBoundName = this.BoundName;
            }

            if (!string.IsNullOrEmpty(this.RhsName))
            {
                model.SelectedRhsName = this.RhsName;
            }

            if (!string.IsNullOrEmpty(this.RangeName))
            {
                model.SelectedRangeName = this.RangeName;
            }

            if (!string.IsNullOrEmpty(this.DefaultVariableBound))
            {
                if (TryParseBound(this.DefaultVariableBound, out Tuple<double, double> bound))
                {
                    model.DefaultVariableBound = bound;
                }
            }

            if (!string.IsNullOrEmpty(this.DefaultConstraintBound))
            {
                if (TryParseBound(this.DefaultConstraintBound, out Tuple<double, double> bound))
                {
                    model.DefaultConstraintBound = bound;
                }
            }

            this.WriteHost($"{model}");
        }

        /// <summary>
        /// Parses a comma separated string into a tuple.
        /// </summary>
        /// <param name="boundStr">The string representation of the tuple.</param>
        /// <param name="bound">The lower and upper bound tuple.</param>
        /// <returns>True if parsed successfully, false otherwise.</returns>
        private static bool TryParseBound(string boundStr, out Tuple<double, double> bound)
        {
            bound = null;

            if (string.IsNullOrEmpty(boundStr))
            {
                return false;
            }

            var parts = boundStr.Split(new[] { ',', ';' });
            if (parts.Length != 2)
            {
                return false;
            }

            if (!double.TryParse(parts[0], out double lower))
            {
                return false;
            }

            if (!double.TryParse(parts[1], out double upper))
            {
                return false;
            }

            bound = new(lower, upper);
            return true;
        }
    }
}
