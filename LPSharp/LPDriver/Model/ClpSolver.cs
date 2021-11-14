// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClpSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using CoinOr.Clp;
    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents CLP solver method and option settings as a single recipe. The options
    /// control starting basis and other settings. Recipes are short hands for the
    /// the individual settings. There is one enumeration for each recipe method in ClpInterface.
    /// </summary>
    public enum ClpRecipe
    {
        /// <summary>
        /// Dual simplex with all-slacks starting basis.
        /// </summary>
        DualSimplex,

        /// <summary>
        /// Dual simplex with crash starting basis.
        /// </summary>
        DualCrash,

        /// <summary>
        /// Primal simplex with automatic starting basis.
        /// </summary>
        PrimalSimplex,

        /// <summary>
        /// Primal simplex with idiot starting basis.
        /// </summary>
        PrimalIdiot,

        /// <summary>
        /// Either primal or dual simplex based on the model. Also create a plus minus
        /// one matrix for efficiency.
        /// </summary>
        Either,

        /// <summary>
        /// Barrier (interior point) method with default Cholesky factorization.
        /// </summary>
        Barrier,
    }

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
            this.Recipe = ClpRecipe.PrimalSimplex;
        }

        /// <summary>
        /// Gets or sets the CLP recipe.
        /// </summary>
        public ClpRecipe Recipe { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public int LogLevel { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"CLP solver {base.ToString()} Recipe={this.Recipe} LogLevel={this.LogLevel}";
        }

        /// <inheritdoc />
        public override void SetParameters(SolverParameters solverParameters)
        {
            if (solverParameters == null)
            {
                return;
            }

            base.SetParameters(solverParameters);
            Utility.SetPropertiesFromList(solverParameters.ClpParameters, this);
        }

        /// <inheritdoc />
        public override void Clear()
        {
            this.clp.Reset();
        }

        /// <inheritdoc />
        public override bool Load(LPModel model)
        {
            if (model == null || !model.IsValid())
            {
                return false;
            }

            var stopwatch = Stopwatch.StartNew();

            // Clear previous model and state.
            this.Clear();
            this.clp.StartModel();

            // Get the lower and upper variable bounds from the model.
            model.GetVariableBounds(
                out SparseVector<string, double> lowerBound,
                out SparseVector<string, double> upperBound);

            // Create solver variables for each column in the model.
            var rows = new Dictionary<string, int>();
            var columns = new Dictionary<string, int>();
            foreach (var colName in model.A.ColumnIndices)
            {
                columns[colName] = this.clp.AddVariable(
                    colName,
                    lowerBound[colName],
                    upperBound[colName]);
                if (columns[colName] == -1)
                {
                    throw new LPSharpException($"Variable {colName} could not be created");
                }
            }

            // Get the lower and upper constraint bounds from the model.
            model.GetConstraintBounds(
                out SparseVector<string, double> lowerLimit,
                out SparseVector<string, double> upperLimit);

            // Create the constraints.
            foreach (var rowName in model.A.Indices)
            {
                var row = model.A[rowName];

                if (rowName == model.Objective)
                {
                    foreach (var colName in row.Indices)
                    {
                        if (!this.clp.SetObjective(colName, row[colName]))
                        {
                            throw new LPSharpException($"Objective column {colName} could not be set");
                        }
                    }
                }
                else
                {
                    rows[rowName] = this.clp.AddConstraint(rowName, lowerLimit[rowName], upperLimit[rowName]);
                    if (rows[rowName] == -1)
                    {
                        throw new LPSharpException($"Constraint {rowName} could not be created");
                    }

                    foreach (var colName in row.Indices)
                    {
                        if (!this.clp.SetCoefficient(rowName, colName, row[colName]))
                        {
                            throw new LPSharpException($"Coefficient row {rowName} column {colName} could not be set");
                        }
                    }
                }
            }

            // Load the model into the solver.
            this.clp.LoadModel();

            stopwatch.Stop();
            this.metrics[LPMetric.LoadTimeMs] = stopwatch.ElapsedMilliseconds;
            this.metrics[LPMetric.ModelName] = model.Name;
            this.metrics[LPMetric.SolverName] = this.Key;

            return true;
        }

        /// <inheritdoc />
        public override bool Solve()
        {
            if (this.TimeLimitInSeconds != 0)
            {
                this.clp.SetMaximumSeconds(this.TimeLimitInSeconds);
            }

            if (this.EnableLogging)
            {
                this.clp.SetLogLevel(this.LogLevel == 0 ? 1 : this.LogLevel);
            }
            else
            {
                this.clp.SetLogLevel(0);
            }

            var stopwatch = Stopwatch.StartNew();

            switch (this.Recipe)
            {
                case ClpRecipe.DualSimplex:
                    this.clp.SolveUsingDualSimplex();
                    break;

                case ClpRecipe.DualCrash:
                    this.clp.SolveUsingDualCrash();
                    break;

                case ClpRecipe.Either:
                    this.clp.SolveUsingEitherSimplex();
                    break;

                case Model.ClpRecipe.PrimalSimplex:
                    this.clp.SolveUsingPrimalSimplex();
                    break;

                case ClpRecipe.PrimalIdiot:
                    this.clp.SolveUsingPrimalIdiot();
                    break;

                case ClpRecipe.Barrier:
                    this.clp.SolveUsingBarrierMethod();
                    break;

                default:
                    this.clp.Solve();
                    break;
            }

            stopwatch.Stop();

            this.ResultStatus = ConvertClpStatusToLPResult(this.clp.Status());
            bool isOptimal = this.ResultStatus == LPResultStatus.Optimal;

            this.metrics[LPMetric.ResultStatus] = this.clp.Status().ToString();
            this.metrics[LPMetric.SolveTimeMs] = stopwatch.ElapsedMilliseconds;

            if (isOptimal)
            {
                this.metrics[LPMetric.Objective] = this.clp.ObjectiveValue();
                this.metrics[LPMetric.Iterations] = this.clp.Iterations();
            }
            else
            {
                this.RemoveMetric(LPMetric.Objective);
                this.RemoveMetric(LPMetric.Iterations);
            }

            Console.WriteLine("Log level = {0}, recipe = {1}, solve time = {2:F0} ms", this.clp.LogLevel(), this.Recipe, this.clp.SolveTimeMs());
            Console.WriteLine("Maximum seconds = {0}, maximum iterations = {1}", this.clp.MaximumSeconds(), this.clp.MaximumIterations());
            Console.WriteLine("Primal tolerance = {0}, dual tolerance = {1}", this.clp.PrimalTolerance(), this.clp.DualTolerance());

            // Just print the first few columns of the objective to verify the methods.
            DoubleVector columnSolutionVec = new();
            DoubleVector reducedCostVec = new();
            DoubleVector objectiveVec = new();
            this.clp.PrimalColumnSolution(columnSolutionVec);
            this.clp.DualColumnSolution(reducedCostVec);
            this.clp.Objective(objectiveVec);

            Console.WriteLine("{0,10} {1,13} {2,13} {3,13}", "ColIndex", "ColSolution", "ReducedCost", "Objective");
            int maxColumns = Math.Min(10, columnSolutionVec.Count);
            for (int i = 0; i < maxColumns; i++)
            {
                Console.WriteLine(
                    "{0,10} {1,13:G7} {2,13:G7} {3,13:G7}",
                    i,
                    columnSolutionVec[i],
                    reducedCostVec[i],
                    objectiveVec[i]);
            }

            return isOptimal;
        }

        /// <inheritdoc />
        public override void WriteModel(string pathName)
        {
            this.clp.WriteMps(pathName);
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