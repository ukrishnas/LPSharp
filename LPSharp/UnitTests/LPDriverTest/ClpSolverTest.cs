// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClpSolverTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using Microsoft.LPSharp.LPDriver.Model;
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
    }
}