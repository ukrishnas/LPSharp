// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecutionResult.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the execution result.
    /// </summary>
    public class ExecutionResult : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResult"/> class.
        /// </summary>
        public ExecutionResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionResult"/> class.
        /// </summary>
        /// <param name="results">The results key value pairs..</param>
        public ExecutionResult(IEnumerable<KeyValuePair<string, object>> results)
            : base()
        {
            if (results != null)
            {
                foreach (var kv in results)
                {
                    this.Add(kv.Key, kv.Value);
                }
            }
        }
    }
}