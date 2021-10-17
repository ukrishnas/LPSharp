// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseVectorTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents unit tests for <see cref="SparseVector{Tindex,Tvalue}"/> class.
    /// </summary>
    [TestClass]
    public class SparseVectorTest
    {
        /// <summary>
        /// Tests a sparse vector with integer keys.
        /// </summary>
        [TestMethod]
        public void SparseVectorIntegerKeyTest()
        {
            var vector = new SparseVector<int, double>();
            const int N = 10;
            var expectElements = new List<double>();
            var expectIndices = new List<int>();
            for (int i = 1; i <= N; i++)
            {
                int index = i * 1000;
                double element = (double)i;
                vector[index] = element;
                expectElements.Add(element);
                expectIndices.Add(index);
            }

            Assert.AreEqual(N, vector.Count, "Vector count");

            var gotElements = vector.ToArray(out int[] gotIndices);
            Assert.IsTrue(expectIndices.SequenceEqual(gotIndices), "Vector indices");
            Assert.IsTrue(expectElements.SequenceEqual(gotElements), "Vector elements");
        }

        /// <summary>
        /// Tests a sparse vector with string keys.
        /// </summary>
        [TestMethod]
        public void SparseVectorStringKeyTest()
        {
            var vector = new SparseVector<string, double>();

            var elements = new Dictionary<string, double>
            {
                { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 4 }, { "e", 5 },
            };

            foreach (var kv in elements)
            {
                vector[kv.Key] = kv.Value;
                Assert.AreEqual(kv.Value, vector[kv.Key]);
            }

            var gotElements = vector.ToArray(out string[] gotIndices);
            Assert.AreEqual(gotElements.Length, elements.Count, "Vector count");
            Assert.IsTrue(elements.Keys.SequenceEqual(gotIndices), "Vector indices");
            Assert.IsTrue(elements.Values.SequenceEqual(gotElements), "Vector elements");
        }
    }
}
