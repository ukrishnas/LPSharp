// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPDriverTest.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriverTest
{
    using LPSharp.LPDriver.Contract;
    using LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents unit tests for <see cref="LPDriver"/> class.
    /// </summary>
    [TestClass]
    public class LPDriverTest
    {
        /// <summary>
        /// Tests model operations in LP driver.
        /// </summary>
        [TestMethod]
        public void LPDriverModelAddGetTest()
        {
            var driver = CreateInstance();

            Assert.IsTrue(driver.AddModel("a", new LPModel()));
            Assert.IsFalse(driver.AddModel(null, new LPModel()));
            Assert.IsNotNull(driver.GetModel("a"));
            Assert.IsNull(driver.GetModel(null));
        }

        /// <summary>
        /// Tests solver operations in LP driver.
        /// </summary>
        [TestMethod]
        public void LPDriverSolverCreateGetTest()
        {
            var driver = CreateInstance();

            Assert.IsNotNull(driver.CreateSolver("glop", SolverType.GLOP));
            Assert.IsNotNull(driver.CreateSolver("clp", SolverType.CLP));

            Assert.IsNull(driver.GetSolver(null));
            Assert.IsNotNull(driver.GetSolver("glop"));
            Assert.IsNotNull(driver.GetSolver("clp"));
            Assert.IsNull(driver.GetSolver("unknown"));
        }

        /// <summary>
        /// Creates a test instance.
        /// </summary>
        /// <returns>An instance of the LP driver.</returns>
        private static ILPDriver CreateInstance()
        {
            return new LPDriver();
        }
    }
}
