// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseMatrixTest.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriverTest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using LPSharp.LPDriver.Model;
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
        /// Tests the column index count and rectangular shape property.
        /// </summary>
        [TestMethod]
        public void SparseMatrixRectangularShapeTest()
        {
            var matrix = new SparseMatrix<int, double>();
            int i = 0;

            // Test tuple is pre-test action and expected rectangular shape, and regular shape.
            foreach (var test in new Tuple<Action, Tuple<int, int>, Tuple<int, int>>[]
            {
                new Tuple<Action, Tuple<int, int>, Tuple<int, int>>(null, new(0, 0), new(0, 0)),

                new Tuple<Action, Tuple<int, int>, Tuple<int, int>>(
                    () =>
                    {
                        matrix[0, 0] = 0.1;
                        matrix[1, 1] = 0.1;
                    }, new(2, 2), new(2, 1)),

                new Tuple<Action, Tuple<int, int>, Tuple<int, int>>(
                    () =>
                    {
                        matrix[0, 10] = 0.1;
                        matrix[2, 30] = 0.1;
                    }, new(3, 4), new(3, 2)),

                new Tuple<Action, Tuple<int, int>, Tuple<int, int>>(
                    () => matrix.Remove(1, 1), new(2, 3), new(2, 2)),
            })
            {
                i++;
                test.Item1?.Invoke();
                Assert.AreEqual(test.Item2, matrix.RectangularShape, $"Rectangular shape test {i}");
                Assert.AreEqual(test.Item3, matrix.Shape, $"Matrix shape test {i}");
            }
        }

        /// <summary>
        /// Tests the row and column iterators.
        /// </summary>
        [TestMethod]
        public void SparseMatrixRowColumnIteratorTest()
        {
            var rowIndices = new List<int>();
            var columnIndices = new HashSet<int>();
            double element = 1;

            var matrix = new SparseMatrix<int, double>();

            for (int i = 0; i < 1000; i += 100)
            {
                rowIndices.Add(i);
                for (int j = 0; j < 100; j += 7)
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
        /// Tests index method with null indices.
        /// </summary>
        [TestMethod]
        public void SparseMatrixNullIndexTest()
        {
            var matrix = new SparseMatrix<string, double>(double.NaN);
            int i = 0;

            // Test tuple is row index, column index, set value, expected get value, and expected has return value.
            foreach (var test in new Tuple<string, string, double, double, bool>[]
            {
                new(null, null, 0.0, double.NaN, false),
                new(null, "col", 0.0, double.NaN, false),
                new("row", null, 0.0, double.NaN, false),
                new("row", "col", 0.0, 0.0, true),
            })
            {
                i++;

                var row = test.Item1;
                var col = test.Item2;
                var value = test.Item3;
                matrix[row, col] = value;

                var expectGet = test.Item4;
                var expectHas = test.Item5;
                Assert.AreEqual(expectGet, matrix[row, col], $"Expected get value, test {i}");
                Assert.AreEqual(expectHas, matrix.Has(row, col), $"Expected has return value, test {i}");
            }
        }

        /// <summary>
        /// Tests the Has() method.
        /// </summary>
        [TestMethod]
        public void SparseMatrixHasTest()
        {
            var imatrix = new SparseMatrix<int, int>();
            imatrix[0, 0] = 1;
            imatrix[100, 100] = 2;
            Assert.IsTrue(imatrix.Has(0, 0));
            Assert.IsTrue(imatrix.Has(100, 100));
            Assert.IsFalse(imatrix.Has(-1, 0));
            Assert.IsFalse(imatrix.Has(100, 0));

            var smatrix = new SparseMatrix<string, int>();
            smatrix[null, "100"] = 1;
            smatrix["100", "100"] = 2;
            Assert.IsFalse(smatrix.Has(null, "100"));
            Assert.IsTrue(smatrix.Has("100", "100"));
            Assert.IsFalse(smatrix.Has("100", null));
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
