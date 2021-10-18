// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlopSolver.cs" company="Microsoft Corporation">
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
        public bool Load(
            LPModel model,
            string boundsName = null,
            string rhsName = null,
            string rangesName = null)
        {
            if (!model.IsValid())
            {
                return false;
            }

            var stopwatch = Stopwatch.StartNew();

            // Get variable lower and upper bounds from the model.
            model.GetVariableBounds(
                boundsName,
                out SparseVector<string, double> lowerBound,
                out SparseVector<string, double> upperBound,
                this.DefaultVariableBounds.Item1,
                this.DefaultVariableBounds.Item2);

            // Get the right hand side lower and upper limits from the model.
            model.GetRhsLimits(
                rhsName,
                rangesName,
                out SparseVector<string, double> lowerLimit,
                out SparseVector<string, double> upperLimit,
                this.DefaultConstraintBounds.Item1,
                this.DefaultConstraintBounds.Item2);

            this.linearSolver.Clear();

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
            Console.WriteLine($"Loaded model {model.Name} in {stopwatch.ElapsedMilliseconds} ms");

            return true;
        }

        /// <inheritdoc />
        public void Solve()
        {
            var stopwatch = Stopwatch.StartNew();

            var parameters = new MPSolverParameters();
            parameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.LP_ALGORITHM,
                (int)MPSolverParameters.LpAlgorithmValues.DUAL);

            var resultStatus = this.linearSolver.Solve();

            stopwatch.Stop();

            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine(
                    "Result not optimal, result = {0}, elapsed = {1} ms",
                    resultStatus,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                Console.WriteLine(
                    "Result = {0}, objective = {1}, iterations={2}, elapsed={3} ms",
                    resultStatus,
                    this.linearSolver.Objective().Value(),
                    this.linearSolver.Iterations(),
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
