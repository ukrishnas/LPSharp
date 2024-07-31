// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPResultStatus.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents the LP result status.
    /// </summary>
    public enum LPResultStatus
    {
        /// <summary>
        /// Unknown, for example the problem is not yet solved.
        /// </summary>
        Unknown,

        /// <summary>
        /// Optimal result.
        /// </summary>
        Optimal,

        /// <summary>
        /// Feasible problem.
        /// </summary>
        Feasible,

        /// <summary>
        /// Infeasible problem.
        /// </summary>
        Infeasible,

        /// <summary>
        /// Unbounded problem.
        /// </summary>
        Unbounded,

        /// <summary>
        /// Stopped problem due to limits exceeded or other events.
        /// </summary>
        Stopped,

        /// <summary>
        /// Other result status.
        /// </summary>
        Other,
    }
}
