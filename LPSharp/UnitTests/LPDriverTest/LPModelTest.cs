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
        /// Tests model set objective method.
        /// </summary>
        [TestMethod]
        public void LPModelSetObjectiveTest()
        {
            var model = new LPModel { Name = "Test" };

            Assert.AreEqual("Test", model.Name, "Name");
            Assert.IsNull(model.Objective, "New model");

            model.RowTypes["cost"] = MpsRow.NoRestriction;
            model.RowTypes["dem1"] = MpsRow.LessOrEqual;
            model.RowTypes["dem2"] = MpsRow.GreaterOrEqual;
            model.RowTypes["cost2"] = MpsRow.NoRestriction;
            model.SetObjective();

            Assert.AreEqual("cost", model.Objective, "Objective");
        }
    }
}
