// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPDriverTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using Microsoft.LPSharp.LPDriver.Contract;
    using Microsoft.LPSharp.LPDriver.Model;
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

            Assert.IsNotNull(driver.CreateSolver("a", SolverType.GLOP));
            Assert.IsNull(driver.CreateSolver("b", SolverType.CLP));
            Assert.IsNotNull(driver.GetSolver("a"));
            Assert.IsNull(driver.GetSolver(null));
            Assert.IsNull(driver.GetSolver("b"));
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
