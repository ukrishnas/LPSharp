// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlopSolverTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using System.Diagnostics;
    using System.Linq;
    using Google.OrTools.LinearSolver;
    using Microsoft.LPSharp.LPDriver.Contract;
    using Microsoft.LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents unit tests for <see cref="GlopSolver"/> class.
    /// </summary>
    [TestClass]
    public class GlopSolverTest
    {
        /// <summary>
        /// Tests solver with example model files.
        /// </summary>
        [TestMethod]
        public void GlopSolverExampleModelTest()
        {
            var solver = new GlopSolver("test");
            TestUtil.TestModels(solver, TestUtil.ExampleModels);
        }

        /// <summary>
        /// Tests solver with WANLP model files.
        /// </summary>
        [TestMethod]
        public void GlopSolverWanlpModelTest()
        {
            var solver = new GlopSolver("test");
            TestUtil.TestModels(solver, TestUtil.WanlpModels);
        }

        /// <summary>
        /// Tests solver with NetLib model files.
        /// </summary>
        [TestMethod]
        public void GlopSolverNetlibModelTest()
        {
            var solver = new GlopSolver("test");
            TestUtil.TestModels(solver, TestUtil.NetlibModels);
        }

        /// <summary>
        /// Tests the native solver API methods.
        /// </summary>
        [TestMethod]
        public void GlopSolverSolverApiTest()
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

            Assert.AreEqual(3, solver.NumVariables(), "Number of variables");
            Assert.AreEqual(3, solver.NumConstraints(), "Number of constraints");

            Solver.ResultStatus resultStatus = solver.Solve();

            Assert.AreEqual(Solver.ResultStatus.OPTIMAL, resultStatus, "Result status");
            TestUtil.AssertAlmostEqual(733.333333, solver.Objective().Value(), "Objective value");
            TestUtil.AssertAlmostEqual(33.333333, x1.SolutionValue(), "x1");
            TestUtil.AssertAlmostEqual(66.666666, x2.SolutionValue(), "x2");
            TestUtil.AssertAlmostEqual(0, x3.SolutionValue(), "x3");

            Assert.AreEqual(2, solver.Iterations(), "Iterations");

            foreach (var x in new[] { x1, x2, x3 })
            {
                Trace.WriteLine(string.Format(
                    "{0} solution={1} reduced cost={2}",
                    x.Name(), x.SolutionValue(), x.ReducedCost()));
            }

            double[] activities = solver.ComputeConstraintActivities();
            foreach (var coeff in new[] { c0, c1, c2 })
            {
                Trace.WriteLine(string.Format(
                    "{0} dual value={1} activities={2}",
                    coeff.Name(), coeff.DualValue(), activities[coeff.Index()]));
            }
        }

        /// <summary>
        /// Tests whether model is correctly input into the solver.
        /// </summary>
        [TestMethod]
        public void GlopSolverModelRoundTripTest()
        {
            var solver = new GlopSolver("test");

            // GLOP solver writes with lower precision than the input model.
            // Hence, using a higher value for tolerance.
            TestUtil.TestModelRoundTrip(
                solver,
                TestUtil.NetlibModels.Select(x => x.Item1).ToArray(),
                MpsFormat.Free,
                1e-2);
        }
    }
}
