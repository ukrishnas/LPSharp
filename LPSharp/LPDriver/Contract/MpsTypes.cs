// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpsTypes.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Contract
{
    using System;

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
        /// </c>Lower bound (b \le x lt +inf).
        /// </summary>
        Lower,

        /// <summary>
        /// Upper bound (-inf \lt x \le b).
        /// </summary>
        Upper,

        /// <summary>
        /// Fixed variable (x = b).
        /// </summary>
        Fixed,

        /// <summary>
        /// Free variable (-inf \lt x \lt +inf).
        /// </summary>
        Free,

        /// <summary>
        /// Lower bound -infinity (-inf \lt x \le 0).
        /// </summary>
        MI,

        /// <summary>
        /// Upper bound +infinity (0 \le x \lt +inf).
        /// </summary>
        PL,

        /// <summary>
        /// Binary variable (x = 0 or 1).
        /// </summary>
        Binary,

        /// <summary>
        /// Integer variable lower bound (b \le x \lt +inf).
        /// </summary>
        LowerInteger,

        /// <summary>
        /// Integer variable upper bound (0 \le x \le b).
        /// </summary>
        UpperInteger,

        /// <summary>
        /// Semi-continuous variable bound (x = 0 or l \le x \le b). If l is not set, then
        /// defaults to 1.
        /// </summary>
        SemiContinuous,
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
                "MI" => MpsBound.MI,
                "PL" => MpsBound.PL,
                "BV" => MpsBound.Binary,
                "LI" => MpsBound.LowerInteger,
                "UI" => MpsBound.UpperInteger,
                "SC" => MpsBound.SemiContinuous,
                _ => null,
            };
        }
    }
}
