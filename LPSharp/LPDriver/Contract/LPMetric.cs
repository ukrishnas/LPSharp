// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPMetric.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriver.Contract
{
    /// <summary>
    /// Represents solver metrics.
    /// </summary>
    public static class LPMetric
    {
        /// <summary>
        /// The load time in milliseconds.
        /// </summary>
        public const string LoadTimeMs = "LoadTimeMs";

        /// <summary>
        /// The solve time in milliseconds.
        /// </summary>
        public const string SolveTimeMs = "SolveTimeMs";

        /// <summary>
        /// The summary information of the solver.
        /// </summary>
        public const string SolverName = "Solver";

        /// <summary>
        /// The summary information of the loaded model.
        /// </summary>
        public const string ModelName = "Model";

        /// <summary>
        /// The result status for the last execution.
        /// </summary>
        public const string ResultStatus = "ResultStatus";

        /// <summary>
        /// The optimal result for the objective.
        /// </summary>
        public const string Objective = "Objective";

        /// <summary>
        /// The number of iterations.
        /// </summary>
        public const string Iterations = "Iterations";
    }
}
