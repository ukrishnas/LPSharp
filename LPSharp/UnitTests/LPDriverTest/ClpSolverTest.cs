// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClpSolverTest.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriverTest
{
    using System.Linq;
    using LPSharp.LPDriver.Contract;
    using LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents unit tests for <see cref="ClpSolver"/> class.
    /// </summary>
    [TestClass]
    public class ClpSolverTest
    {
        /// <summary>
        /// Tests solver with example model files.
        /// </summary>
        [TestMethod]
        public void ClpSolverExampleModelTest()
        {
            var solver = new ClpSolver("test");
            TestUtil.TestModels(solver, TestUtil.ExampleModels);
        }

        /// <summary>
        /// Tests solver with WANLP model files.
        /// </summary>
        [TestMethod]
        public void ClpSolverWanlpModelTest()
        {
            var solver = new ClpSolver("test");
            TestUtil.TestModels(solver, TestUtil.WanlpModels);
        }

        /// <summary>
        /// Tests solver with NetLib model files.
        /// </summary>
        [TestMethod]
        public void ClpSolverNetlibModelTest()
        {
            var solver = new ClpSolver("test");
            TestUtil.TestModels(solver, TestUtil.NetlibModels);
        }

        /// <summary>
        /// Tests whether model is correctly input into the solver.
        /// </summary>
        [TestMethod]
        public void ClpSolverModelRoundTripTest()
        {
            var solver = new ClpSolver("test");
            TestUtil.TestModelRoundTrip(
                solver,
                TestUtil.NetlibModels.Select(x => x.Item1).ToArray(),
                MpsFormat.Fixed);
        }
    }
}