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
            this.mpSolverId = mpSolverId;
            this.linearSolver = Solver.CreateSolver(mpSolverId);
            if (this.linearSolver == null)
            {
                throw new LPSharpException("Unable to create OrTools solver {}", mpSolverId);
            }

            // Set solver properties to default values.
            this.SolveWithParameters = true;
            this.Presolve = true;
            this.Scaling = true;
            this.Incrementality = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to solve with parameters. When unset, it is
        /// as through MP solver common and underlying solver specific parameters are empty.
        /// The default value is true.
        /// </summary>
        public bool SolveWithParameters { get; set; }

        /// <summary>
        /// Gets or sets the LP algorithm. In case a solver does not implement a given algorithm,
        /// the algorithm will be set to the solver default.
        /// </summary>
        public LPAlgorithm LPAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to perform presolve. The default value is true.
        /// Presolve is good to do in most problems.
        /// </summary>
        public bool Presolve { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to perform scaling. The default value is true.
        /// </summary>
        public bool Scaling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to perform incremental solving. The default value is true.
        /// </summary>
        public bool Incrementality { get; set; }

        /// <summary>
        /// Gets or sets the protocol buffer string representation of solver specific parameters.
        /// The protocol buffer string serialization is of the form name: value separated by space
        /// or newline. For example: "use_dual_simplex: 1". See ortools/glop/parameters.proto for
        /// the full list of parameters.
        /// </summary>
        public string SolverSpecificParametersText { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.mpSolverId} solver {base.ToString()}";
        }

        /// <inheritdoc />
        public override void Clear()
        {
            // This clears the extracted model so that the next call to Solve() will be from scratch.
            // This does not reset parameters that were set with SetSolverSpecificParametersAsString()
            // or set_time_limit() or even clear the linear program.
            this.linearSolver.Reset();

            // This clears the linear program offset, all variables and coefficients, and the optimization
            // direction.
            this.linearSolver.Clear();
        }

        /// <inheritdoc />
        public override bool Load(LPModel model)
        {
            if (!model.IsValid())
            {
                return false;
            }

            var stopwatch = Stopwatch.StartNew();

            // Clear previous model and state.
            this.Clear();

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
        public override void SetParameters(SolverParameters solverParameters)
        {
            if (solverParameters?.GlopParameters == null)
            {
                return;
            }

            base.SetParameters(solverParameters);
            Utility.SetPropertiesFromList(solverParameters.GlopParameters.Parameters, this);
            this.SolverSpecificParametersText = solverParameters.GlopParameters.SolverSpecificParameterText;
        }

        /// <inheritdoc />
        public override bool Solve()
        {
            this.metrics[LPMetric.SolverName] = this.Key;

            // Set time limit in linear solver if configured. The linear solver set method takes
            // milliseconds.
            if (this.TimeLimitInSeconds != 0)
            {
                this.linearSolver.SetTimeLimit(this.TimeLimitInSeconds * 1000);
            }

            // If true enable solver logs.
            if (this.EnableLogging)
            {
                this.linearSolver.EnableOutput();
            }
            else
            {
                this.linearSolver.SuppressOutput();
            }

            var stopwatch = Stopwatch.StartNew();

            MPResultStatus resultStatus;
            if (this.SolveWithParameters)
            {
                // Set solver specific parameters. SetSolverSpecificParametersAsString() converts the string
                // back into GlopParameters and updates solver parameters in Solve().
                if (!string.IsNullOrEmpty(this.SolverSpecificParametersText))
                {
                    this.linearSolver.SetSolverSpecificParametersAsString(this.SolverSpecificParametersText);
                }

                // Solve with MP common parameters and the underlying solver specific parameters.
                var mpParameters = this.GetMPSolverParameters();
                resultStatus = this.linearSolver.Solve(mpParameters);
            }
            else
            {
                // Try to unset previously set solver specific parameters.
                this.linearSolver.SetSolverSpecificParametersAsString(string.Empty);

                // Solve the linear program with default parameters.
                resultStatus = this.linearSolver.Solve();
            }

            stopwatch.Stop();

            this.ResultStatus = MPResultToLPResult(resultStatus);
            bool isOptimal = this.ResultStatus == LPResultStatus.Optimal;

            this.metrics[LPMetric.ResultStatus] = resultStatus.ToString();
            this.metrics[LPMetric.SolveTimeMs] = stopwatch.ElapsedMilliseconds;

            if (isOptimal)
            {
                this.metrics[LPMetric.Objective] = this.linearSolver.Objective().Value();
                this.metrics[LPMetric.Iterations] = this.linearSolver.Iterations();
            }
            else
            {
                this.RemoveMetric(LPMetric.Objective);
                this.RemoveMetric(LPMetric.Iterations);
            }

            return isOptimal;
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
                MPResultStatus.OPTIMAL => LPResultStatus.Optimal,
                MPResultStatus.FEASIBLE => LPResultStatus.Feasible,
                MPResultStatus.INFEASIBLE => LPResultStatus.Infeasible,
                MPResultStatus.UNBOUNDED => LPResultStatus.Unbounded,
                MPResultStatus.ABNORMAL => LPResultStatus.Other,
                MPResultStatus.NOT_SOLVED => LPResultStatus.Unknown,
                _ => LPResultStatus.Unknown,
            };
        }

        /// <summary>
        /// Converts LP algorithm to MP integer parameter.
        /// </summary>
        /// <param name="algorithm">The LP algorithm.</param>
        /// <returns>The MP integer parameter.</returns>
        private static int LPAlgorithmToMPIntegerParam(LPAlgorithm algorithm)
        {
            return algorithm switch
            {
                LPAlgorithm.Primal => (int)MPSolverParameters.LpAlgorithmValues.PRIMAL,
                LPAlgorithm.Dual => (int)MPSolverParameters.LpAlgorithmValues.DUAL,
                LPAlgorithm.Barrier => (int)MPSolverParameters.LpAlgorithmValues.BARRIER,
                _ => -1,  // Undefined value means use default.
            };
        }

        /// <summary>
        /// Converts presolve setting to integer parameter.
        /// </summary>
        /// <param name="presolve">The presolve setting.</param>
        /// <returns>The MP integer parameter.</returns>
        private static MPSolverParameters.PresolveValues PresolveToMPIntegerParam(bool presolve)
        {
            return presolve
                ? MPSolverParameters.PresolveValues.PRESOLVE_ON
                : MPSolverParameters.PresolveValues.PRESOLVE_OFF;
        }

        /// <summary>
        /// Converts scaling setting to integer parameter.
        /// </summary>
        /// <param name="scaling">The scaling setting.</param>
        /// <returns>The MP integer parameter.</returns>
        private static MPSolverParameters.ScalingValues ScalingToMPIntegerParam(bool scaling)
        {
            return scaling
                ? MPSolverParameters.ScalingValues.SCALING_ON
                : MPSolverParameters.ScalingValues.SCALING_OFF;
        }

        /// <summary>
        /// Converts incremental setting to integer parameter.
        /// </summary>
        /// <param name="incrementality">The incremental setting.</param>
        /// <returns>The MP integer parameter.</returns>
        private static MPSolverParameters.IncrementalityValues IncrementalityToMPIntegerParam(bool incrementality)
        {
            return incrementality
                ? MPSolverParameters.IncrementalityValues.INCREMENTALITY_ON
                : MPSolverParameters.IncrementalityValues.INCREMENTALITY_OFF;
        }

        /// <summary>
        /// Generates common solver parameters from previously loaded parameters.
        /// </summary>
        /// <returns>The MP solver parameters.</returns>
        private MPSolverParameters GetMPSolverParameters()
        {
            var mpParameters = new MPSolverParameters();

            // Set LP algorithm in MP common parameters.
            if (this.LPAlgorithm != LPAlgorithm.Default)
            {
                mpParameters.SetIntegerParam(
                        MPSolverParameters.IntegerParam.LP_ALGORITHM,
                        LPAlgorithmToMPIntegerParam(this.LPAlgorithm));
            }

            // Set whether presolve should be enabled.
            mpParameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.PRESOLVE,
                (int)PresolveToMPIntegerParam(this.Presolve));

            // Set whether scaling should be on.
            mpParameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.SCALING,
                (int)ScalingToMPIntegerParam(this.Scaling));

            // Set whether incremental solving should be on.
            mpParameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.INCREMENTALITY,
                (int)IncrementalityToMPIntegerParam(this.Incrementality));

            return mpParameters;
        }
    }
}
