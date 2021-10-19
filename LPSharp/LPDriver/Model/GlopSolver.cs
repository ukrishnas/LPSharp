// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlopSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Google.OrTools.LinearSolver;
    using Microsoft.LPSharp.LPDriver.Contract;

    using MPResultStatus = Google.OrTools.LinearSolver.Solver.ResultStatus;

    /// <summary>
    /// Represents the interface implementation for GLOP solver.
    /// </summary>
    public class GlopSolver : LPSolverAbstract, ILPInterface
    {
        /// <summary>
        /// The LP solver object.
        /// </summary>
        private readonly Solver linearSolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlopSolver"/> class.
        /// </summary>
        /// <param name="key">The solver key.</param>
        public GlopSolver(string key)
            : base(key)
        {
            // The parameter is a string representation of MPSolver optimization problem type.
            this.linearSolver = Solver.CreateSolver("GLOP_LINEAR_PROGRAMMING");
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"GLOP solver {base.ToString()}";
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
            model.GetRhsBounds(
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

            var parameters = new MPSolverParameters();
            parameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.LP_ALGORITHM,
                (int)MPSolverParameters.LpAlgorithmValues.DUAL);

            var resultStatus = this.linearSolver.Solve();

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
                this.RemoveMetric("Iterations");
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
