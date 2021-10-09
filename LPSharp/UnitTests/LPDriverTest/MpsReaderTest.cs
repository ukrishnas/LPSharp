// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpsReaderTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.LPSharp.LPDriver.Model;

    /// <summary>
    /// Unit tests for <see cref="MpsReader"/> class.
    /// </summary>
    [TestClass]
    public class MpsReaderTest
    {
        /// <summary>
        /// Tests MPS read operation with a simple test problem.
        /// </summary>
        [TestMethod]
        public void MpsReadSimpleTest()
        {
            var reader = new MpsReader();
            foreach (var test in new[] { "test1.mps", "test1.mps.gz" })
            {
                var filename = $"TestData\\{test}";
                reader.Read(filename);
                Assert.AreEqual(0, reader.Errors.Count, $"Read errors {filename}");
            }
        }

        /// <summary>
        /// Tests MPS read operation with COIN-OR hello example.
        /// </summary>
        [TestMethod]
        public void MpsReadCoinTest()
        {
            var reader = new MpsReader();
            foreach (var test in new[] { "hello.mps" })
            {
                var filename = $"TestData\\{test}";
                reader.Read(filename);
                Assert.AreEqual(0, reader.Errors.Count, $"Read errors {filename}");
            }
        }

        /// <summary>
        /// Tests MPS read operation with OR-tools examples.
        /// </summary>
        [TestMethod]
        public void MpsReadOrtoolsTest()
        {
            var reader = new MpsReader();
            foreach (var test in new[] { "test2.mps", "test3.mps" })
            {
                var filename = $"TestData\\{test}";
                reader.Read(filename);
                Assert.AreEqual(0, reader.Errors.Count, $"Read errors {filename}");
            }
        }
    }
}
