﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlopSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using Google.OrTools.LinearSolver;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents the interface implementation for GLOP solver.
    /// </summary>
    public class GlopSolver : ILPInterface
    {
        private readonly Solver linearSolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlopSolver"/> class.
        /// </summary>
        public GlopSolver()
        {
            this.linearSolver = Solver.CreateSolver("GLOP");
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "GLOP solver";
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

            // Get variable lower and upper bounds from the model.
            model.GetBounds(
                boundsName,
                out SparseVector<string, double> lowerBound,
                out SparseVector<string, double> upperBOund);

            // Get the right hand side lower and upper limits from the model.
            model.GetRhsLimits(
                rhsName,
                rangesName,
                out SparseVector<string, double> lowerLimit,
                out SparseVector<string, double> upperLimit);

            this.linearSolver.Reset();

            // Create solver variables for each column in the model.
            var x = new Dictionary<string, Variable>();
            foreach (var colIndex in model.A.ColumnIndices)
            {
                x[colIndex] = this.linearSolver.MakeNumVar(
                    lowerBound[colIndex],
                    upperBOund[colIndex],
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

            return false;
        }

        /// <inheritdoc />
        public void Solve()
        {
            var parameters = new MPSolverParameters();
            parameters.SetIntegerParam(
                MPSolverParameters.IntegerParam.LP_ALGORITHM,
                (int)MPSolverParameters.LpAlgorithmValues.DUAL);

            Solver.ResultStatus resultStatus = this.linearSolver.Solve(parameters);

            // Check that the problem has an optimal solution.
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("The problem does not have an optimal solution!");
                return;
            }

            Console.WriteLine("Optimal objective value = {0}", this.linearSolver.Objective().Value());
            Console.WriteLine("Solver iterations = {0}", this.linearSolver.Iterations());
            Console.WriteLine("Solver wall time = {0} ms", this.linearSolver.WallTime());
        }
    }
}
