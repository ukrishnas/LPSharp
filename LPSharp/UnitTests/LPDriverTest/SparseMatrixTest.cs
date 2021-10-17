// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseMatrixTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriverTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Represents unit tests for <see cref="SparseMatrix{Tindex,Tvalue}"/> class.
    /// </summary>
    [TestClass]
    public class SparseMatrixTest
    {
        /// <summary>
        /// Tests matrix shape and count properties.
        /// </summary>
        [TestMethod]
        public void SparseMatrixShapeTest()
        {
            var matrix = new SparseMatrix<int, double>();
            int i = 0;

            foreach (var test in new Tuple<Action, int, Tuple<int, int>>[]
            {
                // Empty matrix.
                new Tuple<Action, int, Tuple<int, int>>(null, 0, new(0, 0)),

                // Add 3 elements.
                new Tuple<Action, int, Tuple<int, int>>(
                    () =>
                    {
                        matrix[20, -10] = 1.3;
                        matrix[30, 15] = 2.5;
                        matrix[1000, -200] = 3.7;
                    },
                    3,
                    new Tuple<int, int>(3, 1)),

                // Add a fourth element.
                new Tuple<Action, int, Tuple<int, int>>(() => matrix[30, 16] = 2.5, 4, new(3, 2)),
            })
            {
                i++;
                test.Item1?.Invoke();
                Assert.AreEqual(test.Item2, matrix.Count, $"Matrix count in test {i}");
                Assert.AreEqual(test.Item3, matrix.Shape, $"Matrix shape in test {i}");
            }
        }

        /// <summary>
        /// Tests the row and column iterators.
        /// </summary>
        [TestMethod]
        public void SparseMatrixRowColumnIteratorTest()
        {
            var rowIndices = new List<double>();
            var columnIndices = new HashSet<double>();
            double element = 1;

            var matrix = new SparseMatrix<double, double>();

            for (double i = 0; i < 1; i += 0.1)
            {
                rowIndices.Add(i);
                for (double j = 0; j < i; j += 0.05)
                {
                    matrix[i, j] = element;
                    element += 1;
                    columnIndices.Add(j);
                }
            }

            Trace.WriteLine($"Row indices = {string.Join(", ", matrix.RowIndices)}");
            Assert.IsTrue(rowIndices.SequenceEqual(matrix.RowIndices), "Row indices");

            Trace.WriteLine($"Column indices = {string.Join(", ", matrix.ColumnIndices)}");
            Assert.IsTrue(columnIndices.SetEquals(matrix.ColumnIndices), "Column indices");
        }

        /// <summary>
        /// Tests the remove element method.
        /// </summary>
        [TestMethod]
        public void SparseMatrixRemoveTest()
        {
            var matrix = new SparseMatrix<int, int>();
            int i = 0;

            // Test tuple is pre-test action, remove row index, remove column index,
            // expected remove result, and expected count.
            foreach (var test in new Tuple<Action, int, int, bool, Tuple<int, int>>[]
            {
                // Remove from an empty vector.
                new Tuple<Action, int, int, bool, Tuple<int, int>>(null, 0, 0, false, new(0, 0)),

                // Add 3 elements and remove one.
                new Tuple<Action, int, int, bool, Tuple<int, int>>(
                    () =>
                    {
                        matrix[100, 100] = 1;
                        matrix[200, 100] = 2;
                        matrix[200, 200] = 3;
                    }, 200, 100, true, new(2, 1)),

                // Removes last element of a row.
                new Tuple<Action, int, int, bool, Tuple<int, int>>(null, 200, 200, true, new(1, 1)),
            })
            {
                i++;
                test.Item1?.Invoke();
                var success = matrix.Remove(test.Item2, test.Item3);

                Assert.AreEqual(test.Item4, success, $"Remove return value test {i}");
                Assert.AreEqual(test.Item5, matrix.Shape, $"Matrix shape test {i}");
            }
        }
    }
}
