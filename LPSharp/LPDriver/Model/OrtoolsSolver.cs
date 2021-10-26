// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrtoolsSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Google.OrTools.LinearSolver;
    using Microsoft.LPSharp.LPDriver.Contract;

    using MPResultStatus = Google.OrTools.LinearSolver.Solver.ResultStatus;

    /// <summary>
    /// Represents a generic Google OR-Tools linear solver.
    /// </summary>
    public class OrtoolsSolver : LPSolverAbstract
    {
        /// <summary>
        /// The LP solver object.
        /// </summary>
        protected readonly Solver linearSolver;

        /// <summary>
        /// The string representation of the optimization problem type.
        /// </summary>
        protected readonly string mpSolverId;

        /// <summary>
        /// The MP solver parameters.
        /// </summary>
        protected MPSolverParameters mpParameters;

        /// <summary>
        /// A value indicating whether the solver parameters have been changed by user.
        /// </summary>
        private bool defaultParameters = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoolsSolver"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        /// <param name="mpSolverId">The string representation of the optimization problem type.</param>
        public OrtoolsSolver(string key, string mpSolverId)
            : base(key)
        {
            if (string.IsNullOrEmpty(mpSolverId) ||
                !(mpSolverId.Equals("GLOP_LINEAR_PROGRAMMING", StringComparison.Ordinal) ||
                mpSolverId.Equals("CLP_LINEAR_PROGRAMMING", StringComparison.Ordinal)))
            {
                throw new LPSharpException("Unsupported OrTools solver {0}", mpSolverId);
            }

            // Create the Google OrTools solver. The parameter is a string representation of
            // the desired optimization problem type.
            this.linearSolver = Solver.CreateSolver(mpSolverId);
            if (this.linearSolver == null)
            {
                throw new LPSharpException("Unable to create OrTools solver {}", mpSolverId);
            }

            this.mpSolverId = mpSolverId;

            // Create empty parameters with default values.
            this.mpParameters = new MPSolverParameters();
            this.defaultParameters = true;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.mpSolverId} solver {base.ToString()}";
        }

        /// <inheritdoc />
        public override void Reset()
        {
            // This clears the linear program offset, all variables and coefficients, and the optimization
            // direction.
            this.linearSolver.Clear();

            // This clears the extracted model so that the next call to Solve() will be from scratch.
            // This does not reset parameters that were set with SetSolverSpecificParametersAsString()
            // or set_time_limit() or even clear the linear program.
            this.linearSolver.Reset();
        }

        /// <inheritdoc />
        public override bool Load(LPModel model)
        {
            if (!model.IsValid())
            {
                return false;
            }

            var stopwatch = Stopwatch.StartNew();

            // Clear  offset, all variables and coefficients, and the optimization
            // direction.
            this.linearSolver.Clear();

            // Get the lower and upper variable bounds from the model.
            model.GetVariableBounds(
                out SparseVector<string, double> lowerBound,
                out SparseVector<string, double> upperBound);

            // Create solver variables for each column in the model.
            var x = new Dictionary<string, Variable>();
            foreach (var colIndex in model.A.ColumnIndices)
            {
                x[colIndex] = this.linearSolver.MakeNumVar(
                    lowerBound[colIndex],
                    upperBound[colIndex],
                    colIndex);
            }

            // Create the objective.
            var objective = this.linearSolver.Objective();
            var row = model.A[model.Objective];
            foreach (var colIndex in row.Indices)
            {
                objective.SetCoefficient(x[colIndex], row[colIndex]);
            }

            // Get the lower and upper constraint bounds from the model.
            model.GetConstraintBounds(
                out SparseVector<string, double> lowerLimit,
                out SparseVector<string, double> upperLimit);

            // Create the constraints.
            foreach (var rowIndex in model.A.Indices)
            {
                if (rowIndex == model.Objective)
                {
                    continue;
                }

                var constraint = this.linearSolver.MakeConstraint(lowerLimit[rowIndex], upperLimit[rowIndex]);

                row = model.A[rowIndex];
                foreach (var colIndex in row.Indices)
                {
                    constraint.SetCoefficient(x[colIndex], row[colIndex]);
                }
            }

            stopwatch.Stop();
            this.metrics[LPMetric.LoadTimeMs] = stopwatch.ElapsedMilliseconds;
            this.metrics[LPMetric.ModelName] = model.Name;

            return true;
        }

        /// <inheritdoc />
        public override void Set(SolverParameter name, params object[] values)
        {
            int integerValue;

            this.defaultParameters = false;

            switch (name)
            {
                case SolverParameter.DualSimplex:
                    this.mpParameters.SetIntegerParam(
                        MPSolverParameters.IntegerParam.LP_ALGORITHM,
                        (int)MPSolverParameters.LpAlgorithmValues.DUAL);
                    integerValue = this.mpParameters.GetIntegerParam(MPSolverParameters.IntegerParam.LP_ALGORITHM);
                    Trace.WriteLine($"LP algorithm = {integerValue} (DUAL), GLOP solver may return abnormal or not solved status");
                    break;

                case SolverParameter.PrimalSimplex:
                    this.mpParameters.SetIntegerParam(
                        MPSolverParameters.IntegerParam.LP_ALGORITHM,
                        (int)MPSolverParameters.LpAlgorithmValues.PRIMAL);
                    integerValue = this.mpParameters.GetIntegerParam(MPSolverParameters.IntegerParam.LP_ALGORITHM);
                    Trace.WriteLine($"LP algorithm = {integerValue} (PRIMAL)");
                    break;

                case SolverParameter.TimeLimitInSeconds:
                    if (values?.Length == 1 && values[0] is int timeLimit)
                    {
                        this.linearSolver.SetTimeLimit(timeLimit * 1000);
                        Console.WriteLine("Time limit = {0} ms", timeLimit * 1000);
                    }

                    break;

                default:
                    // There is no case for barrier method because GLOP does not support it.
                    Trace.WriteLine($"Unsupported solver parameter {name}");
                    break;
            }

            return;
        }

        /// <inheritdoc />
        public override bool Solve()
        {
            this.metrics[LPMetric.SolverName] = this.Key;

            var stopwatch = Stopwatch.StartNew();

            // Note that non-default parameters may make the problem unsolvable, or abnormal. Please
            // be aware of what you are doing.
            if (!this.defaultParameters)
            {
                Console.WriteLine("Warning, non-default parameters may be in use!");
            }

            // Solve the linear program.
            var resultStatus = this.linearSolver.Solve(this.mpParameters);

            stopwatch.Stop();

            this.metrics[LPMetric.SolveTimeMs] = stopwatch.ElapsedMilliseconds;
            this.metrics[LPMetric.ResultStatus] = MPResultToLPResult(resultStatus);

            if (resultStatus == MPResultStatus.OPTIMAL)
            {
                this.metrics[LPMetric.Objective] = this.linearSolver.Objective().Value();
                this.metrics[LPMetric.Iterations] = this.linearSolver.Iterations();
            }
            else
            {
                this.RemoveMetric(LPMetric.Objective);
                this.RemoveMetric(LPMetric.Iterations);
            }

            return resultStatus == MPResultStatus.OPTIMAL;
        }

        /// <summary>
        /// Converts GLOP result to standard LP result.
        /// </summary>
        /// <param name="result">The MP result.</param>
        /// <returns>The LP result.</returns>
        private static LPResultStatus MPResultToLPResult(MPResultStatus result)
        {
            return result switch
            {
                MPResultStatus.OPTIMAL => LPResultStatus.OPTIMAL,
                MPResultStatus.FEASIBLE => LPResultStatus.FEASIBLE,
                MPResultStatus.INFEASIBLE => LPResultStatus.INFEASIBLE,
                MPResultStatus.UNBOUNDED => LPResultStatus.UNBOUNDED,
                MPResultStatus.ABNORMAL => LPResultStatus.ABNORMAL,
                MPResultStatus.NOT_SOLVED => LPResultStatus.NOT_SOLVED,
                _ => LPResultStatus.UNDEFINED,
            };
        }
    }
}
