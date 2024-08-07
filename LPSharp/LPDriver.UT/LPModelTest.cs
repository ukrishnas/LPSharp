﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LPModelTest.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriverTest
{
    using System;
    using System.Collections.Generic;
    using LPSharp.LPDriver.Contract;
    using LPSharp.LPDriver.Model;
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

        /// <summary>
        /// Tests setting default bounds.
        /// </summary>
        [TestMethod]
        public void LPModelDefaultBoundTest()
        {
            var model = new LPModel();
            int i = 0;

            // Test tuple is set value, and expected get value.
            foreach (var test in new Tuple<Tuple<double, double>, Tuple<double, double>>[]
            {
                new(new(0, 0), new(0, 0)),
                new(new(1, 0), new(0, 0)),
                new(new(-1, 1), new(-1, 1)),
                new(new(double.NegativeInfinity, double.PositiveInfinity), new(double.NegativeInfinity, double.PositiveInfinity)),
            })
            {
                i++;
                model.DefaultVariableBound = test.Item1;
                Assert.AreEqual(test.Item2, model.DefaultVariableBound, "Variable bound test {i}");

                model.DefaultConstraintBound = test.Item1;
                Assert.AreEqual(test.Item2, model.DefaultConstraintBound, "Constraint bound test {i}");
            }
        }

        /// <summary>
        /// Tests the selected RHS name property.
        /// </summary>
        [TestMethod]
        public void LPModelRhsNameTest()
        {
            var model = new LPModel();

            model.SelectedRhsName = "RHS";
            Assert.IsNull(model.SelectedRhsName, "Selected name set with invalid name");
            model.B["RHS", "r1"] = 1;
            model.SelectedRhsName = "RHS";
            Assert.AreEqual("RHS", model.SelectedRhsName, "Selected name set with valid name");

            model.B["RHS2", "r1"] = 2;
            Assert.AreEqual(2, model.RhsNames.Count, "Count of available names");
        }

        /// <summary>
        /// Tests the selected bound name property.
        /// </summary>
        [TestMethod]
        public void LPModelBoundNameTest()
        {
            var model = new LPModel();

            model.SelectedBoundName = "Bound";
            Assert.IsNull(model.SelectedBoundName, "Selected name set with invalid name");

            model.SetBound("Bound", "c1", MpsBound.Fixed, 1);
            model.SelectedBoundName = "Bound";
            Assert.AreEqual("Bound", model.SelectedBoundName, "Selected name set with valid name");

            model.SetBound("Bound2", "c1", MpsBound.Fixed, 1);
            Assert.AreEqual(2, model.BoundNames.Count, "Count of available names");
        }

        /// <summary>
        /// Tests the selected range name property.
        /// </summary>
        [TestMethod]
        public void LPModelRangeNameTest()
        {
            var model = new LPModel();

            model.SelectedBoundName = "Range";
            Assert.IsNull(model.SelectedRangeName, "Selected name set with invalid name");

            model.R["Range", "r1"] = 1;
            model.SelectedRangeName = "Range";
            Assert.AreEqual("Range", model.SelectedRangeName, "Selected name set with valid name");

            model.R["Range2", "r1"] = 1;
            Assert.AreEqual(2, model.RangeNames.Count, "Count of available names");
        }

        /// <summary>
        /// Tests the set and get variable bound methods.
        /// </summary>
        [TestMethod]
        public void LPModelVariableBoundTest()
        {
            const string Name = "bound";

            var model = new LPModel();
            Assert.AreEqual(
                new(0, double.PositiveInfinity),
                model.DefaultVariableBound,
                "Default variable bound");

            model.SetBound(Name, "c1", MpsBound.Lower, 5);
            model.SetBound(Name, "c2", MpsBound.Upper, 5);
            model.SetBound(Name, "c1", MpsBound.Upper, 10);
            model.SetBound(Name, "c3", MpsBound.Fixed, 1);
            model.SetBound(Name, "c4", MpsBound.Free, 0);
            model.SetBound(Name, "c5", MpsBound.MinusInfinity, 0);
            model.SetBound(Name, "c6", MpsBound.PlusInfinity, 0);

            var lower = new SparseVector<string, double>(
                new Dictionary<string, double>
                {
                    { "c1", 5 },
                    { "c2", 0 },
                    { "c3", 1 },
                    { "c4", 0 },
                    { "c5", double.NegativeInfinity },
                    { "c6", 0 },
                });
            Assert.AreEqual(lower, model.L[Name], "Lower bound");

            var upper = new SparseVector<string, double>(
                new Dictionary<string, double>
                {
                    { "c1", 10 },
                    { "c2", 5 },
                    { "c3", 1 },
                    { "c4", double.PositiveInfinity },
                    { "c5", double.PositiveInfinity },
                    { "c6", double.PositiveInfinity },
                });
            Assert.AreEqual(upper, model.U[Name], "Upper bound");

            // Need to create a row in the constraint matrix since the generate bounds
            // method only generates bounds for columns in the constraint matrix. Also, the
            // upper bound default will be model's default upper bound.
            upper.Default = double.PositiveInfinity;
            foreach (var colIndex in upper.Indices)
            {
                model.A["row1", colIndex] = -1;
            }

            model.GetVariableBounds(
                out SparseVector<string, double> lowerBound, out SparseVector<string, double> upperBound);
            Assert.AreEqual(lower, lowerBound, "Get variable lower bound");
            Assert.AreEqual(upper, upperBound, "Get variable upper bound");
        }

        /// <summary>
        /// Tests get constraint bound methods.
        /// </summary>
        [TestMethod]
        public void LPModelConstraintBoundTest()
        {
            const string Name = "RHS";

            var model = new LPModel();
            Assert.AreEqual(
                new(double.NegativeInfinity, double.PositiveInfinity),
                model.DefaultConstraintBound,
                "Default constraint bound");

            // Set up coefficients, row types, and right hand side values.
            model.A["r1", "c1"] = -1;
            model.A["r2", "c2"] = -1;
            model.A["r3", "c1"] = -1;

            model.RowTypes["r1"] = MpsRow.Equal;
            model.RowTypes["r2"] = MpsRow.LessOrEqual;
            model.RowTypes["r3"] = MpsRow.GreaterOrEqual;

            model.B[Name, "r1"] = 5;
            model.B[Name, "r2"] = -5;

            model.GetConstraintBounds(
                out SparseVector<string, double> lowerBound,
                out SparseVector<string, double> upperBound);

            // Verify lower bound.
            Assert.AreEqual(
                new SparseVector<string, double>(
                    new Dictionary<string, double>
                    {
                        { "r1", 5 },
                        { "r3", 0 },
                    },
                    double.NegativeInfinity),
                lowerBound, "Constraint lower bound");

            // Verify upper bound.
            Assert.AreEqual(
                new SparseVector<string, double>(
                    new Dictionary<string, double>
                    {
                        { "r1", 5 },
                        { "r2", -5 },
                    },
                    double.PositiveInfinity),
                upperBound, "Constraint upper bound");
        }

        /// <summary>
        /// Tests the constraint bound with range.
        /// </summary>
        [TestMethod]
        public void LPModelConstraintBoundWithRangeTest()
        {
            const string Name = "Name";

            var model = new LPModel();
            model.A["r1", "c1"] = -1;
            model.A["r2", "c2"] = -1;
            model.A["r3", "c1"] = -1;

            model.RowTypes["r1"] = MpsRow.Equal;
            model.RowTypes["r2"] = MpsRow.LessOrEqual;
            model.RowTypes["r3"] = MpsRow.GreaterOrEqual;

            model.B[Name, "r1"] = 5;
            model.B[Name, "r2"] = -5;
            model.B[Name, "r3"] = 15;

            model.R[Name, "r1"] = 10;
            model.R[Name, "r2"] = 20;
            model.R[Name, "r3"] = 30;

            model.GetConstraintBounds(
                out SparseVector<string, double> lowerBound,
                out SparseVector<string, double> upperBound);
            model.UpdateConstraintBoundsWithRange(lowerBound, upperBound);

            // Verify lower bound.
            Assert.AreEqual(
                new SparseVector<string, double>(
                    new Dictionary<string, double>
                    {
                        { "r1", 5 },
                        { "r2", -25 },
                        { "r3", 15 },
                    },
                    double.NegativeInfinity),
                lowerBound, "Constraint lower bound");

            // Verify upper bound.
            Assert.AreEqual(
                new SparseVector<string, double>(
                    new Dictionary<string, double>
                    {
                        { "r1", 15 },
                        { "r2", -5 },
                        { "r3", 45 },
                    },
                    double.PositiveInfinity),
                upperBound, "Constraint upper bound");
        }

        /// <summary>
        /// Checks the validity of models.
        /// </summary>
        [TestMethod]
        public void LPModelIsValidTest()
        {
            var model = new LPModel();
            Assert.IsFalse(model.IsValid());

            model.A["r1", "c1"] = -1;
            model.RowTypes["r1"] = MpsRow.NoRestriction;
            model.SetObjective();
        }
    }
}
