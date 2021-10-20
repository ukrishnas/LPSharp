// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestUtil.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using System;
    using System.IO;
    using Microsoft.LPSharp.LPDriver.Contract;
    using Microsoft.LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents unit test utilities.
    /// </summary>
    public static class TestUtil
    {
        /// <summary>
        /// The default tolerance for double precision comparisons.
        /// </summary>
        public const double Tolerance = 1e-5;

        /// <summary>
        /// Example models. The fields are model file and objective value.
        /// </summary>
        public static Tuple<string, double>[] ExampleModels =
        {
            new("afiro.mps", -464.753142857143),
            new("hello.mps", 0),
            new("test1.mps", 54),
            new("test2.mps", 3.23684210526316),
        };

        /// <summary>
        /// Asserts whether two double precision numbers are almost equal.
        /// </summary>
        /// <param name="expect">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The assert message.</param>
        /// <param name="tolerance">The tolerance for comparison.</param>
        public static void AssertAlmostEqual(
            double expect,
            double actual,
            string message = null,
            double tolerance = Tolerance)
        {
            var assertMessage = $"{message} abs({expect} - {actual}) > {tolerance}";
            Assert.IsTrue(Math.Abs(expect - actual) < tolerance, assertMessage);
        }

        /// <summary>
        /// Tests a solver on models.
        /// </summary>
        /// <param name="solver">The solver.</param>
        /// <param name="testModels">The test models, fields are model file and objective value.</param>
        /// <param name="testFolder">The test folder.</param>
        public static void TestModels(
            ILPInterface solver,
            Tuple<string, double>[] testModels,
            string testFolder = "TestData")
        {
            var reader = new MpsReader();

            var solverAbstract = solver as LPSolverAbstract;
            Assert.IsNotNull(solverAbstract, $"{solver} cast as abstract");

            foreach (var test in testModels)
            {
                var filename = $"{testFolder}\\{test.Item1}";
                Assert.IsTrue(File.Exists(filename));

                var model = reader.Read(filename);
                model.Name = Path.GetFileNameWithoutExtension(filename);

                Assert.IsTrue(solver.Load(model), $"{solver} {model.Name} load status");
                Assert.IsTrue(solver.Solve(), "{solver} {model.Name} optimal result status");

                TestUtil.AssertAlmostEqual(
                    test.Item2,
                    (double)solverAbstract.Metrics[LPMetric.Objective],
                    $"{solver} {model.Name} objective");
            }
        }
    }
}
