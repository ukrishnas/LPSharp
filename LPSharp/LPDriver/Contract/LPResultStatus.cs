// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPResultStatus.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents the LP result status.
    /// </summary>
    public enum LPResultStatus
    {
        /// <summary>
        /// Problem not yet solved.
        /// </summary>
        NOT_SOLVED,

        /// <summary>
        /// Optimal result.
        /// </summary>
        OPTIMAL,

        /// <summary>
        /// Feasible problem.
        /// </summary>
        FEASIBLE,

        /// <summary>
        /// Infeasible problem.
        /// </summary>
        INFEASIBLE,

        /// <summary>
        /// Unbounded problem.
        /// </summary>
        UNBOUNDED,

        /// <summary>
        /// Abnormal problem.
        /// </summary>
        ABNORMAL,

        /// <summary>
        /// Undefined result status.
        /// </summary>
        UNDEFINED,
    }
}
