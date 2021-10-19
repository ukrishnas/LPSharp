// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPSolverAbstract.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an abstract class for an LP solver. It holds methods and properties
    /// that are common to all solvers.
    /// </summary>
    public abstract class LPSolverAbstract
    {
        /// <summary>
        /// The solver metrics;
        /// </summary>
        protected Dictionary<string, object> metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPSolverAbstract"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public LPSolverAbstract(string key)
        {
            this.Key = key;
            this.metrics = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the solver key.
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Gets a copy of the solver metrics.
        /// </summary>
        public ExecutionResult Metrics => new(this.metrics);

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Key={this.Key}";
        }

        /// <summary>
        /// Removes a metric.
        /// </summary>
        /// <param name="key">The metric key.</param>
        protected void RemoveMetric(string key)
        {
            if (this.metrics.ContainsKey(key))
            {
                this.metrics.Remove(key);
            }
        }
    }
}
