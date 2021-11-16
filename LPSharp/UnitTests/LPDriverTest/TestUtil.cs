// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestUtil.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using System;
    using System.Diagnostics;
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
        public const double Tolerance = 1e-3;

        /// <summary>
        /// Example models from COIN-OR and GLOP examples. The fields are model file
        /// and objective value.
        /// </summary>
        public static Tuple<string, double>[] ExampleModels =
        {
            new("afiro.mps", -464.753142857143),
            new("hello.mps", 0),
            new("test1.mps", 54),
            new("test2.mps", 3.23684210526316),

            // This file uses all real number MPS features but is infeasible.
            new("test3.mps", double.NaN),
        };

        /// <summary>
        /// Example models in free MPS format.
        /// </summary>
        public static Tuple<string, double>[] FreeFormatModels =
        {
            // This file is a snippet of a larger model in free format.
            new("testfree.mps", double.NaN),
        };

        /// <summary>
        /// WANLPv2 benchmark models that are small in size. The fields are model file
        /// and objective value.
        /// </summary>
        public static Tuple<string, double>[] WanlpModels =
        {
            new("wander-primal1.mps", -1590.63821067608),
            new("wander-primal2.mps", -4247098.879760965),
        };

        /// <summary>
        /// NetLib models from https://www.cuter.rl.ac.uk/Problems/netlib.shtml.
        /// The fields are model file and objective value.
        /// </summary>
        public static Tuple<string, double>[] NetlibModels =
        {
            new("25fv47.mps", 5501.8458882867135),
            new("addlittle.mps", 225494.9631623804),
            new("afiro.mps", -464.753142857143),
            new("agg2.mps", -20239252.355977114),
            new("boeing1.mps", -335.21356750712664),
            new("boeing2.mps", -315.0187280152029),
            new("ken-07.mps", -679520443.381687),
            new("pds-02.mps", 28857862010),

            // This file has trailing whitespace.
            new("stocfor3.mps", -39976.78394364959),
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
            var solverKey = solverAbstract.Key;

            foreach (var test in testModels)
            {
                var filename = $"{testFolder}\\{test.Item1}";
                var expectObjective = test.Item2;

                // Ignore files that do not have an optimal result.
                if (double.IsNaN(expectObjective))
                {
                    continue;
                }

                Assert.IsTrue(File.Exists(filename), $"{filename} not present");

                Trace.WriteLine($"Testing with {filename}");

                var model = reader.Read(filename);
                model.Name = Path.GetFileNameWithoutExtension(filename);

                Assert.IsTrue(solver.Load(model), $"{solverKey} {model.Name} load status");
                Assert.IsTrue(solver.Solve(), $"{solverKey} {model.Name} optimal result status");

                AssertAlmostEqual(
                    expectObjective,
                    (double)solverAbstract.Metrics[LPMetric.Objective],
                    $"{solverKey} {model.Name} objective");
            }
        }

        /// <summary>
        /// Tests whether the model that is loaded into the solver, and then written
        /// out by the solver is the same as the original model.
        /// </summary>
        /// <param name="solver">The solver.</param>
        /// <param name="testModels">The test models.</param>
        /// <param name="format">The format of MPS file written by solver.</param>
        /// <param name="tolerance">The tolerance for model comparison.</param>
        /// <param name="testFolder">The test folder.</param>
        public static void TestModelRoundTrip(
            ILPInterface solver,
            string[] testModels,
            MpsFormat format = MpsFormat.Free,
            double tolerance = LPConstant.DefaultTolerance,
            string testFolder = "TestData")
        {
            var reader = new MpsReader();

            var solverAbstract = solver as LPSolverAbstract;
            Assert.IsNotNull(solverAbstract, $"{solver} cast as abstract");
            var solverKey = solverAbstract.Key;

            var roundTripDirName = Guid.NewGuid().ToString();
            var roundTripDirectory = Directory.CreateDirectory(roundTripDirName);
            Trace.WriteLine($"Round trip directory {roundTripDirName}");

            var modelComparer = new LPModelComparer { Tolerance = tolerance, };

            foreach (var testModel in testModels)
            {
                var modelFilePath = $"{testFolder}\\{testModel}";
                var model = reader.Read(modelFilePath);
                model.Name = Path.GetFileNameWithoutExtension(modelFilePath);

                Assert.IsTrue(solver.Load(model), $"{solverKey} {model.Name} load status");

                var roundTripModelFilePath = Path.Combine(roundTripDirectory.FullName, testModel);
                solver.WriteModel(roundTripModelFilePath);
                Assert.IsTrue(File.Exists(roundTripModelFilePath), $"Round trip model file not found {roundTripModelFilePath}");

                var roundTripModel = reader.Read(roundTripModelFilePath, format);
                var result = modelComparer.Compare(model, roundTripModel);

                // To see differences.
                if (result != 0)
                {
                    int maxDifferences = Math.Min(10, modelComparer.Differences.Count);
                    for (int i = 0; i < maxDifferences; i++)
                    {
                        Trace.WriteLine(modelComparer.Differences[i]);
                    }
                }

                Assert.AreEqual(0, result, $"{solverKey} {model.Name} model round trip result");
            }
        }
    }
}
