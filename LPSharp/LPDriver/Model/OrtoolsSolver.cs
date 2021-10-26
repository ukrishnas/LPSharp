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
    public class OrtoolsSolver : LPSolverAbstract, ILPInterface
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

            // Create an empty parameters and set dual method as default.
            this.mpParameters = this.DualSimplexParameters();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.mpSolverId} solver {base.ToString()}";
        }

        /// <inheritdoc />
        public bool Load(LPModel model)
        {
            if (!model.IsValid())
            {
                return false;
            }

            var stopwatch = Stopwatch.StartNew();

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
        public bool Solve()
        {
            this.metrics[LPMetric.SolverName] = this.Key;

            var stopwatch = Stopwatch.StartNew();

            // Set 1 minute time limit.
            this.linearSolver.SetTimeLimit(60 * 1000);

            // Reset solver internal state.
            this.linearSolver.Reset();

            // Solve.
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

        /// <inheritdoc />
        public void Set(SolverParameter parameter, params object[] arguments)
        {
            switch (parameter)
            {
                case SolverParameter.DualSimplex:
                    this.mpParameters = this.DualSimplexParameters();
                    break;

                case SolverParameter.PrimalSimplex:
                    this.mpParameters = this.PrimalSimplexParameters();
                    break;

                default:
                    throw new LPSharpException("Unsupported solver parameter {0}", parameter);
            }

            return;
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

        /// <summary>
        /// Sets dual simplex parameters.
        /// </summary>
        /// <returns>The MP parameters. </returns>
        private MPSolverParameters DualSimplexParameters()
        {
            var parameters = new MPSolverParameters();
            parameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.LP_ALGORITHM,
                (int)MPSolverParameters.LpAlgorithmValues.DUAL);
            return parameters;
        }

        /// <summary>
        /// Sets primal simplex parameters.
        /// </summary>
        /// <returns>The MP parameters. </returns>
        private MPSolverParameters PrimalSimplexParameters()
        {
            var parameters = new MPSolverParameters();
            parameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.LP_ALGORITHM,
                (int)MPSolverParameters.LpAlgorithmValues.PRIMAL);
            return parameters;
        }
    }
}
