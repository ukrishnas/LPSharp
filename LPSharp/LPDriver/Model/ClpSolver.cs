// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClpSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Diagnostics;
    using CoinOr.Clp;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents the interface implementation for CLP solver.
    /// </summary>
    public class ClpSolver : LPSolverAbstract
    {
        /// <summary>
        /// The CLP solver. This is a wrapper around the ClpSimplex driver, ClpSolve options,
        /// and other related classes.
        /// </summary>
        private readonly ClpInterface clp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClpSolver"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public ClpSolver(string key)
            : base(key)
        {
            this.clp = new ClpInterface();

            // Default values.
            this.SolveType = SolveType.Dual;
        }

        /// <summary>
        /// Gets or sets the solve type.
        /// </summary>
        public SolveType SolveType { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public int LogLevel { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"CLP solver {base.ToString()}";
        }

        /// <inheritdoc />
        public override void Clear()
        {
            this.clp.Reset();
        }

        /// <inheritdoc />
        public override bool Load(LPModel model)
        {
            // TODO: need to convert string to SWIG string. Also ReadMps() is not the right method.
            this.clp.ReadMps(null);
            return true;
        }

        /// <inheritdoc />
        public override void SetParameters(SolverParameters solverParameters)
        {
            if (solverParameters == null)
            {
                return;
            }

            Utility.SetPropertiesFromList(solverParameters.ClpParameters, this);
        }

        /// <inheritdoc />
        public override bool Solve()
        {
            this.clp.SetMaximumSeconds(this.TimeLimitInSeconds);
            if (this.EnableLogging)
            {
                this.clp.SetLogLevel(this.LogLevel == 0 ? 1 : this.LogLevel);
            }
            else
            {
                this.clp.SetLogLevel(0);
            }

            var stopwatch = Stopwatch.StartNew();

            switch (this.SolveType)
            {
                case SolveType.Dual:
                    this.clp.SolveUsingDualSimplex();
                    break;

                case SolveType.Either:
                    this.clp.SolveUsingEitherSimplex();
                    break;

                case SolveType.Primal:
                    this.clp.SolveUsingPrimalSimplex();
                    break;

                case SolveType.Barrier:
                    this.clp.SolveUsingBarrierMethod();
                    break;

                default:
                    // Unknown or unsupported solve type.
                    return false;
            }

            stopwatch.Stop();

            this.ResultStatus = ConvertClpStatusToLPResult(this.clp.Status());
            bool isOptimal = this.ResultStatus == LPResultStatus.Optimal;

            this.metrics[LPMetric.ResultStatus] = this.clp.Status().ToString();
            this.metrics[LPMetric.SolveTimeMs] = stopwatch.ElapsedMilliseconds;

            if (isOptimal)
            {
                this.metrics[LPMetric.Objective] = this.clp.Objective();
                this.metrics[LPMetric.Iterations] = this.clp.Iterations();
            }
            else
            {
                this.RemoveMetric(LPMetric.Objective);
                this.RemoveMetric(LPMetric.Iterations);
            }

            return isOptimal;
        }

        /// <summary>
        /// Converts CLP problem status to standard LP result.
        /// </summary>
        /// <param name="status">The solve status.</param>
        /// <returns>The LP result status.</returns>
        private static LPResultStatus ConvertClpStatusToLPResult(ClpStatus status)
        {
            return status switch
            {
                ClpStatus.Unknown => LPResultStatus.Unknown,
                ClpStatus.DualFeasible => LPResultStatus.Feasible,
                ClpStatus.PrimalFeasible => LPResultStatus.Feasible,
                ClpStatus.Optimal => LPResultStatus.Optimal,
                ClpStatus.StoppedDueToErrors => LPResultStatus.Stopped,
                ClpStatus.StoppedDueToLimits => LPResultStatus.Stopped,
                ClpStatus.StoppedByEventHandler => LPResultStatus.Stopped,
                _ => LPResultStatus.Other,
            };
        }
    }
}