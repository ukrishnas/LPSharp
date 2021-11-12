// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPSolverAbstract.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents an abstract class for an LP solver. It holds methods and properties
    /// that are common to all solvers.
    /// </summary>
    public abstract class LPSolverAbstract : ILPInterface
    {
        /// <summary>
        /// The solver metrics;
        /// </summary>
        protected SortedDictionary<string, object> metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPSolverAbstract"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public LPSolverAbstract(string key)
        {
            this.Key = key;
            this.metrics = new SortedDictionary<string, object>(new MetricComparer());
        }

        /// <summary>
        /// Gets the solver key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets or sets the time limit in seconds. The solver is interrupted when this
        /// time limit is exceeded and the objective result at that point is returned.
        /// </summary>
        public int TimeLimitInSeconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log solver actions as it searches
        /// for the optimal result. This is useful for debugging purposes but should be
        /// turned off for performance measurement.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets the result status from the last execution.
        /// </summary>
        public LPResultStatus ResultStatus { get; protected set; }

        /// <summary>
        /// Gets a copy of the solver metrics.
        /// </summary>
        public ExecutionResult Metrics => new(this.metrics);

        /// <inheritdoc />
        public virtual void SetParameters(SolverParameters solverParameters)
        {
            if (solverParameters == null)
            {
                return;
            }

            Utility.SetPropertiesFromList(solverParameters.GenericParameters, this);
        }

        /// <inheritdoc />
        public abstract bool Load(LPModel model);

        /// <inheritdoc />
        public abstract void Clear();

        /// <inheritdoc />
        public abstract bool Solve();

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

        /// <summary>
        /// Represents a custom comparer for metric keys so that metrics output
        /// in a more readable order.
        /// </summary>
        private class MetricComparer : IComparer<string>
        {
            /// <summary>
            /// Represents a custom comparer for metric keys.
            /// </summary>
            /// <param name="a">The first metric.</param>
            /// <param name="b">The second metric.</param>
            /// <returns>The comparison result.</returns>
            public int Compare(string a, string b)
            {
                Func<string, string> mangle = x =>
                {
                    return x == LPMetric.ModelName ? $"0_{x}" :
                        x == LPMetric.SolverName ? $"1_{x}" :
                        x == LPMetric.SolveTimeMs ? $"2_{x}" :
                        x == LPMetric.Objective ? $"3_{x}" :
                        x;
                };

                a = mangle(a);
                b = mangle(b);

                return a.CompareTo(b);
            }
        }
    }
}
