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
            const int N = 10;
            var expectElements = new List<double>();
            var expectIndices = new List<int>();

            var vector = new SparseVector<int, double>();

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
            var elements = new Dictionary<string, double>
            {
                { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 4 }, { "e", 5 },
            };

            var vector = new SparseVector<string, double>();

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

        /// <summary>
        /// Tests vector index and element iterators;
        /// </summary>
        [TestMethod]
        public void SparseVectorIteratorTest()
        {
            var elements = new Dictionary<string, double>
            {
                { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 4 }, { "e", 5 },
            };

            var vector = new SparseVector<string, double>();

            foreach (var kv in elements)
            {
                vector[kv.Key] = kv.Value;
            }

            foreach (var index in vector.Indices)
            {
                Assert.IsTrue(elements.ContainsKey(index), $"Index {index} should be present");
                Assert.AreEqual(elements[index], vector[index], $"Element at index {index} should be equal");
                Assert.IsTrue(vector.Has(index), "Vector should have index {index}");
            }

            Assert.IsTrue(vector.Elements.SequenceEqual(elements.Values), "Element iterator should return same sequence");
        }

        /// <summary>
        /// Tests basic operations on a vector whose elements are themselves sparse vectors.
        /// </summary>
        [TestMethod]
        public void SparseVectorOfVectorsTest()
        {
            var matrix = new SparseVector<int, SparseVector<int, double>>();
            var expectElements = new List<double>();
            for (int i = 1; i < 5; i++)
            {
                int rowindex = i * 100;
                matrix[rowindex] = new SparseVector<int, double>();
                for (int j = 0; j < 5; j++)
                {
                    int colindex = j * 1000;
                    double element = i * j;
                    matrix[rowindex][colindex] = element;
                    expectElements.Add(element);
                }
            }

            var rows = matrix.ToArray(out _);
            int index = 0;
            foreach (var rowVec in rows)
            {
                var elements = rowVec.ToArray(out _);
                foreach (var element in elements)
                {
                    Assert.AreEqual(expectElements[index++], element, $"Element {index}");
                }
            }
        }
    }
}
