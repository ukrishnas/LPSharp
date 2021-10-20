// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpsTypes.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    using System;

    /// <summary>
    /// Represents the MPS file format.
    /// </summary>
    public enum MpsFormat
    {
        /// <summary>
        /// Fixed format. Fields are in fixed column positions.
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// Free format. Fields do not adhere to fixed format column positions.
        /// </summary>
        Free,
    }

    /// <summary>
    /// Represents the MPS section names verbatim, except for case.
    /// </summary>
    public enum MpsSection
    {
        /// <summary>
        /// Name section.
        /// </summary>
        Name,

        /// <summary>
        /// Row section.
        /// </summary>
        Rows,

        /// <summary>
        /// Column section.
        /// </summary>
        Columns,

        /// <summary>
        /// Right hand side section.
        /// </summary>
        Rhs,

        /// <summary>
        /// Bounds section.
        /// </summary>
        Bounds,

        /// <summary>
        /// Ranges section.
        /// </summary>
        Ranges,

        /// <summary>
        /// End of data.
        /// </summary>
        Endata,
    }

    /// <summary>
    /// Represents the type of row in the constraint matrix.
    /// </summary>
    public enum MpsRow
    {
        /// <summary>
        /// Equality.
        /// </summary>
        Equal,

        /// <summary>
        /// Less than or equal.
        /// </summary>
        LessOrEqual,

        /// <summary>
        /// Greater than or equal.
        /// </summary>
        GreaterOrEqual,

        /// <summary>
        /// Objective or no restriction.
        /// </summary>
        NoRestriction,
    }

    /// <summary>
    /// Represents the bound on variable.
    /// </summary>
    public enum MpsBound
    {
        /// <summary>
        /// Lower bound.
        /// </summary>
        Lower,

        /// <summary>
        /// Upper bound.
        /// </summary>
        Upper,

        /// <summary>
        /// Fixed variable.
        /// </summary>
        Fixed,

        /// <summary>
        /// Free variable.
        /// </summary>
        Free,

        /// <summary>
        /// Minus infinity lower bound.
        /// </summary>
        MinusInfinity,

        /// <summary>
        /// Plus infinity upper bound.
        /// </summary>
        PlusInfinity,
    }

    /// <summary>
    /// Represents methods related to MPS types.
    /// </summary>
    public static class MpsTypes
    {
        /// <summary>
        /// Parses a value into a section.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The section or null.</returns>
        public static MpsSection? ParseSection(string value)
        {
            if (!Enum.TryParse(value, ignoreCase: true, out MpsSection section))
            {
                return null;
            }

            return section;
        }

        /// <summary>
        /// Parses a value into a row type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The row type or null.</returns>
        public static MpsRow? ParseRow(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value switch
            {
                "E" => MpsRow.Equal,
                "G" => MpsRow.GreaterOrEqual,
                "L" => MpsRow.LessOrEqual,
                "N" => MpsRow.NoRestriction,
                _ => null,
            };
        }

        /// <summary>
        /// Parses a value into a bound type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The bound type or null.</returns>
        public static MpsBound? ParseBound(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value switch
            {
                "LO" => MpsBound.Lower,
                "UP" => MpsBound.Upper,
                "FX" => MpsBound.Fixed,
                "FR" => MpsBound.Free,
                "MI" => MpsBound.MinusInfinity,
                "PL" => MpsBound.PlusInfinity,
                _ => null,
            };
        }
    }
}
