// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseVectorTest.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriverTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LPSharp.LPDriver.Model;
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

            var expect = new SparseVector<string, double>(elements);
            Assert.AreEqual(expect, vector);
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

        /// <summary>
        /// Tests vector index and element iterators.
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
        /// Tests index method with null index.
        /// </summary>
        [TestMethod]
        public void SparseVectorNullIndexTest()
        {
            var vec = new SparseVector<string, double>();
            vec[null] = 0.0;
            Assert.IsTrue(vec[null] == 0.0, "Null index get");
            Assert.IsFalse(vec.Has(null), "Null index Has()");
        }

        /// <summary>
        /// Tests the Has() method.
        /// </summary>
        [TestMethod]
        public void SparseVectorHasTest()
        {
            var ivec = new SparseVector<int, double>();
            ivec[1] = 1.1;
            ivec[2] = 2.2;
            Assert.IsFalse(ivec.Has(0));
            Assert.IsTrue(ivec.Has(1));
            Assert.IsTrue(ivec.Has(2));
            Assert.IsFalse(ivec.Has(3));

            var svec = new SparseVector<string, double>();
            svec[null] = 0;
            svec["1"] = 1.1;
            svec["2"] = 2.2;
            Assert.IsFalse(svec.Has(null));
            Assert.IsTrue(svec.Has("1"));
            Assert.IsTrue(svec.Has("2"));
            Assert.IsFalse(svec.Has("3"));
        }

        /// <summary>
        /// Tests the remove element method.
        /// </summary>
        [TestMethod]
        public void SparseVectorRemoveElementTest()
        {
            var vector = new SparseVector<int, int>();
            int i = 0;

            // Test tuple is pre-test action, remove index, expected remove result, and expected count.
            foreach (var test in new Tuple<Action, int, bool, int>[]
            {
                // Remove from an empty vector.
                new Tuple<Action, int, bool, int>(null, 0, false, 0),

                // Add 3 elements and remove one.
                new Tuple<Action, int, bool, int>(
                    () => vector[100] = vector[200] = vector[300] = 1, 200, true, 2),

                // Remove an element that is not present.
                new Tuple<Action, int, bool, int>(null, 400, false, 2),
            })
            {
                i++;
                test.Item1?.Invoke();
                var success = vector.Remove(test.Item2);

                Assert.AreEqual(test.Item3, success, $"Remove return value test {i}");
                Assert.AreEqual(test.Item4, vector.Count, $"Vector count test {i}");
            }
        }

        /// <summary>
        /// Tests the default value property.
        /// </summary>
        [TestMethod]
        public void SparseVectorDefaultValueTest()
        {
            var ivec = new SparseVector<int, int>(int.MaxValue);
            Assert.AreEqual(int.MaxValue, ivec[0], "Integer MaxValue default");

            var dvec = new SparseVector<int, double>(double.PositiveInfinity);
            Assert.AreEqual(double.PositiveInfinity, dvec[0], "Double positive infinity default");
            dvec.Default = 0;
            Assert.AreEqual(0, dvec[0], "Double zero default");

            var nvec = new SparseVector<int, double?>();
            Assert.IsNull(nvec[0], "Nullable double default");
            nvec.Default = double.NaN;
            Assert.AreEqual(double.NaN, nvec[0], "Nullable NaN default");
        }

        /// <summary>
        /// Tests the initialization of a zero vector.
        /// </summary>
        [TestMethod]
        public void SparseVectorZeroVectorTest()
        {
            var explicitZero = new SparseVector<int, double>(0);
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, explicitZero[i], $"Explicit zero {i}");
            }

            var implicitZero = new SparseVector<int, double>();
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, implicitZero[i], $"Implicit zero {i}");
            }
        }

        /// <summary>
        /// Tests the clone method.
        /// </summary>
        [TestMethod]
        public void SparseVectorCloneTest()
        {
            var vec = new SparseVector<int, int>(int.MaxValue);
            var elements = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                vec[i * 10] = i * 100;
                elements.Add(i * 100);
            }

            var clone = vec.Clone();
            Assert.AreEqual(10, clone.Count, "Clone count");
            Assert.AreEqual(clone, vec, "Clone equals original vector");

            for (int i = 10; i < 15; i++)
            {
                vec[i * 10] = i * 100;
            }

            Assert.AreEqual(10, clone.Count, "Clone count should not change");
            Assert.AreNotEqual(clone, vec, "Clone should not equal changed vector");
        }

        /// <summary>
        /// Tests the get hash code method.
        /// </summary>
        [TestMethod]
        public void SparseVectorHashCodeTest()
        {
            var hashCodes = new HashSet<int>();

            var vec = new SparseVector<string, double>();

            foreach (var test in new Tuple<Action>[]
            {
                new(() => vec.Default = 1),
                new(() => vec["a"] = 1.0),
                new(() => vec["b"] = 2.0),
            })
            {
                test.Item1();
                Assert.IsTrue(hashCodes.Add(vec.GetHashCode()));
            }

            Assert.IsFalse(hashCodes.Add(vec.GetHashCode()));
        }

        /// <summary>
        /// Tests the equals method.
        /// </summary>
        [TestMethod]
        public void SparseVectorEqualsTest()
        {
            var a = new SparseVector<int, double>(new Dictionary<int, double>
            {
                { 1, 1.0 },
                { 2, 2.0 },
            });

            var b = a.Clone();

            Assert.IsTrue(a.Equals(a), "Vector equals itself");
            Assert.IsFalse(a.Equals(null), "Vector does not equal null");
            Assert.IsTrue(a.Equals(b), "Vector equals clone");
            Assert.IsTrue(Equals(a, b as object), "Vector equals clone object");

            a.Default = double.PositiveInfinity;
            Assert.IsFalse(a.Equals(b), "Unequal vectors after changing default in one");
            b.Default = double.PositiveInfinity;
            Assert.IsTrue(a.Equals(b), "Equal vectors after changing default in both");

            b[3] = 3.0;
            Assert.IsFalse(a.Equals(b), "Unequal vectors after addition to one");
            a[3] = 3.0;
            Assert.IsTrue(a.Equals(b), "Equal vectors after addition to both");

            a.Remove(1);
            Assert.IsFalse(a.Equals(b), "Unequal vectors after removal from one");
            b.Remove(1);
            Assert.IsTrue(a.Equals(b), "Equals vectors after removal from both");
        }
    }
}
