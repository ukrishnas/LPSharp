// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSolver.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using Google.OrTools.LinearSolver;

    public static class TestSolver
    {
        /// <summary>
        /// Tests GLOP solver.
        /// </summary>
        public static void TestGlop()
        {
            Solver solver = Solver.CreateSolver("GLOP");

            // x1, x2 and x3 are continuous non-negative variables.
            Variable x1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x1");
            Variable x2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x2");
            Variable x3 = solver.MakeNumVar(0.0, double.PositiveInfinity, "x3");

            // Maximize 10 * x1 + 6 * x2 + 4 * x3.
            Objective objective = solver.Objective();
            objective.SetCoefficient(x1, 10);
            objective.SetCoefficient(x2, 6);
            objective.SetCoefficient(x3, 4);
            objective.SetMaximization();

            // x1 + x2 + x3 <= 100.
            Constraint c0 = solver.MakeConstraint(double.NegativeInfinity, 100.0);
            c0.SetCoefficient(x1, 1);
            c0.SetCoefficient(x2, 1);
            c0.SetCoefficient(x3, 1);

            // 10 * x1 + 4 * x2 + 5 * x3 <= 600.
            Constraint c1 = solver.MakeConstraint(double.NegativeInfinity, 600.0);
            c1.SetCoefficient(x1, 10);
            c1.SetCoefficient(x2, 4);
            c1.SetCoefficient(x3, 5);

            // 2 * x1 + 2 * x2 + 6 * x3 <= 300.
            Constraint c2 = solver.MakeConstraint(double.NegativeInfinity, 300.0);
            c2.SetCoefficient(x1, 2);
            c2.SetCoefficient(x2, 2);
            c2.SetCoefficient(x3, 6);

            Console.WriteLine("Number of variables = " + solver.NumVariables());
            Console.WriteLine("Number of constraints = " + solver.NumConstraints());

            Solver.ResultStatus resultStatus = solver.Solve();

            // Check that the problem has an optimal solution.
            if (resultStatus != Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("The problem does not have an optimal solution!");
                return;
            }

            Console.WriteLine("Problem solved in " + solver.WallTime() + " milliseconds");

            // The objective value of the solution.
            Console.WriteLine("Optimal objective value = " + solver.Objective().Value());

            // The value of each variable in the solution.
            Console.WriteLine("x1 = " + x1.SolutionValue());
            Console.WriteLine("x2 = " + x2.SolutionValue());
            Console.WriteLine("x3 = " + x3.SolutionValue());

            Console.WriteLine("Advanced usage:");
            double[] activities = solver.ComputeConstraintActivities();

            Console.WriteLine("Problem solved in " + solver.Iterations() + " iterations");
            Console.WriteLine("x1: reduced cost = " + x1.ReducedCost());
            Console.WriteLine("x2: reduced cost = " + x2.ReducedCost());
            Console.WriteLine("x3: reduced cost = " + x3.ReducedCost());
            Console.WriteLine("c0: dual value = " + c0.DualValue());
            Console.WriteLine("    activity = " + activities[c0.Index()]);
            Console.WriteLine("c1: dual value = " + c1.DualValue());
            Console.WriteLine("    activity = " + activities[c1.Index()]);
            Console.WriteLine("c2: dual value = " + c2.DualValue());
            Console.WriteLine("    activity = " + activities[c2.Index()]);
        }
    }
}
