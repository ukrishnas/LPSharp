// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPModelTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using Microsoft.LPSharp.LPDriver.Contract;
    using Microsoft.LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents unit tests for <see cref="LPModel"/> class.
    /// </summary>
    [TestClass]
    public class LPModelTest
    {
        /// <summary>
        /// Tests simple LP model creation.
        /// </summary>
        [TestMethod]
        public void LPModelAddRowTest()
        {
            var model = new LPModel { Name = "Test" };
            model.AddRow("cost", MpsRow.NoRestriction);
            model.AddRow("dem1", MpsRow.LessOrEqual);
            model.AddRow("dem2", MpsRow.GreaterOrEqual);

            Assert.AreEqual("Test", model.Name, "Name");
            Assert.AreEqual("cost", model.Objective, "Objective");
        }
    }
}
