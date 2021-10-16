﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpsReader.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.LPSharp.LPDriver.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    using Microsoft.LPSharp.LPDriver.Contract;

    /// <summary>
    /// Represents a reader of mathematical programming system fixed MPS format files.
    /// </summary>
    public class MpsReader
    {
        /// <summary>
        /// Fixed MPS format field position and length for section headers.
        /// </summary>
        private static readonly Tuple<int, int>[] SectionFieldPositions = new Tuple<int, int>[]
        {
            new Tuple<int, int>(1, 12),
            new Tuple<int, int>(15, 8),
        };

        /// <summary>
        /// Fixed MPS format field position and length for rows and bounds sections.
        /// </summary>
        private static readonly Tuple<int, int>[] RowFieldPositions = new Tuple<int, int>[]
        {
            new Tuple<int, int>(2, 2),
            new Tuple<int, int>(5, 8),
            new Tuple<int, int>(15, 8),
            new Tuple<int, int>(25, 14),
        };

        /// <summary>
        /// Fixed MPS format field position and length for columns, RHS, and ranges sections.
        /// </summary>
        private static readonly Tuple<int, int>[] ColumnFieldPositions = new Tuple<int, int>[]
        {
            new Tuple<int, int>(5, 8),
            new Tuple<int, int>(15, 8),
            new Tuple<int, int>(25, 14),
            new Tuple<int, int>(40, 8),
            new Tuple<int, int>(50, 14),
        };

        /// <summary>
        /// The errors while reading the file.
        /// </summary>
        private readonly List<string> errors;

        /// <summary>
        /// The last RHS name.
        /// </summary>
        private string lastRhsName = "Default";

        /// <summary>
        /// The last ranges name.
        /// </summary>
        private string lastRangesName = "Default";

        /// <summary>
        /// The last bounds name.
        /// </summary>
        private string lastBoundsName = "Default";

        /// <summary>
        /// Initializes a new instance of the <see cref="MpsReader"/> class.
        /// </summary>
        public MpsReader()
        {
            this.errors = new List<string>();
        }

        /// <summary>
        /// Gets the errors while parsing.
        /// </summary>
        public IReadOnlyList<string> Errors => this.errors;

        /// <summary>
        /// Reads a mathematical programming system (MPS) file in fixed MPS format.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>The linear programming model.</returns>
        public LPModel Read(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(nameof(filename));
            }

            LPModel model = new();
            this.errors.Clear();

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var gzipped = filename.EndsWith("gz");
                if (gzipped)
                {
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        this.ParseMps(gzipStream, model);
                    }
                }
                else
                {
                    this.ParseMps(stream, model);
                }
            }

            return model;
        }

        /// <summary>
        /// Parses an MPS stream.
        /// </summary>
        /// <param name="stream">The MPS stream.</param>
        /// <param name="model">The linear programming model.</param>
        private void ParseMps(Stream stream, LPModel model)
        {
            string line;
            MpsSection? section = null;

            var reader = new StreamReader(stream);
            while ((line = reader.ReadLine()) != null)
            {
                var fields = this.ParseLine(line, section, out bool sectionLine);
                if (fields == null)
                {
                    continue;
                }

                if (sectionLine)
                {
                    // Process section header fields.
                    section = MpsTypes.ParseSection(fields[0]);
                    if (section == null)
                    {
                        this.errors.Add($"Invalid section {fields[0]} in {line}");
                        continue;
                    }

                    if (section == MpsSection.Endata)
                    {
                        // If end of data, then stop reading further lines.
                        break;
                    }
                    else if (section == MpsSection.Name)
                    {
                        // If name section, then read the name of the problem.
                        model.Name = fields[1];
                    }

                    continue;
                }

                if (section == MpsSection.Rows)
                {
                    this.ParseRow(fields, model);
                }
                else if (section == MpsSection.Columns)
                {
                    this.ParseColumn(fields, model);
                }
                else if (section == MpsSection.Rhs)
                {
                    this.ParseRhs(fields, model);
                }
                else if (section == MpsSection.Bounds)
                {
                    this.ParseBound(fields, model);
                }
                else if (section == MpsSection.Ranges)
                {
                    this.ParseRange(fields, model);
                }
                else
                {
                    this.errors.Add($"Ignored line in section {section} {line}");
                }
            }
        }

        /// <summary>
        /// Parses a line into fields.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="section">The section.</param>
        /// <param name="sectionLine">If true, parsed a section header (output parameter).</param>
        /// <returns>The list of fields.</returns>
        private IList<string> ParseLine(string line, MpsSection? section, out bool sectionLine)
        {
            // Return null for comment line.
            if (string.IsNullOrEmpty(line) || line[0] == '*')
            {
                sectionLine = false;
                return null;
            }

            sectionLine = line[0] != ' ';
            var fields = new List<string>();

            Tuple<int, int>[] positions;
            if (sectionLine || section == null)
            {
                positions = SectionFieldPositions;
            }
            else if (section == MpsSection.Rows || section == MpsSection.Bounds)
            {
                positions = RowFieldPositions;
            }
            else
            {
                positions = ColumnFieldPositions;
            }

            foreach (var position in positions)
            {
                // Note that position starts from 1, while string index starts from 0.
                var startIndex = position.Item1 - 1;
                var length = position.Item2;

                if (startIndex >= line.Length)
                {
                    break;
                }
                else if (startIndex + length >= line.Length)
                {
                    length = line.Length - startIndex;
                }

                var field = line.Substring(startIndex, length).Trim();
                fields.Add(field);
            }

            return fields;
        }

        /// <summary>
        /// Parses a row.
        /// </summary>
        /// <param name="fields">The fields in the line.</param>
        /// <param name="model">The linear programming model.</param>
        private void ParseRow(IList<string> fields, LPModel model)
        {
            if (fields.Count != 2)
            {
                this.errors.Add($"Invalid number of row fields {fields.Count} in {string.Join(' ', fields)}");
                return;
            }

            var type = MpsTypes.ParseRow(fields[0]);
            if (type == null)
            {
                this.errors.Add($"Unparsable row type {fields[0]}");
                return;
            }

            var name = fields[1];
            model.AddRow(name, type.Value);
        }

        /// <summary>
        /// Parses a column.
        /// </summary>
        /// <param name="fields">The fields in the line.</param>
        /// <param name="model">The linear programming model.</param>
        private void ParseColumn(IList<string> fields, LPModel model)
        {
            if (fields.Count != 3 && fields.Count != 5)
            {
                this.errors.Add($"Invalid number of column fields {fields.Count} in {string.Join(' ', fields)}");
                return;
            }

            var columnName = fields[0];
            var rowName = fields[1];
            if (double.TryParse(fields[2], out double value))
            {
                model.AddCoefficient(columnName, rowName, value);
            }
            else
            {
                this.errors.Add($"Unparseable column coefficient {fields[2]} in {string.Join(' ', fields)}");
            }

            if (fields.Count == 5)
            {
                rowName = fields[3];
                if (double.TryParse(fields[4], out value))
                {
                    model.AddCoefficient(columnName, rowName, value);
                }
                else
                {
                    this.errors.Add($"Unparseable column coefficient {fields[4]} in {string.Join(' ', fields)}");
                }
            }
        }

        /// <summary>
        /// Parses a right hand side.
        /// </summary>
        /// <param name="fields">The fields in the line.</param>
        /// <param name="model">The linear programming model.</param>
        private void ParseRhs(IList<string> fields, LPModel model)
        {
            if (fields.Count != 3 && fields.Count != 5)
            {
                this.errors.Add($"Invalid number of RHS fields {fields.Count} in {string.Join(' ', fields)}");
                return;
            }

            var rhsName = fields[0];
            var rowName = fields[1];

            // If RHS name is null, then use the last name. I think it is possible for the name to be
            // ommitted in subsequent lines once it is set.
            if (string.IsNullOrEmpty(rhsName))
            {
                rhsName = this.lastRhsName;
            }
            else
            {
                this.lastRhsName = rhsName;
            }

            if (double.TryParse(fields[2], out double coefficient))
            {
                model.AddRhs(rhsName, rowName, coefficient);
            }
            else
            {
                this.errors.Add($"Unparseable RHS coefficient {fields[2]} in {string.Join(' ', fields)}");
            }

            if (fields.Count == 5)
            {
                rowName = fields[3];
                if (double.TryParse(fields[4], out coefficient))
                {
                    model.AddRhs(rhsName, rowName, coefficient);
                }
                else
                {
                    this.errors.Add($"Unparseable RHS coefficient {fields[4]} in {string.Join(' ', fields)}");
                }
            }
        }

        /// <summary>
        /// Parses a bound.
        /// </summary>
        /// <param name="fields">The fields in the line.</param>
        /// <param name="model">The linear programming model.</param>
        private void ParseBound(IList<string> fields, LPModel model)
        {
            var type = MpsTypes.ParseBound(fields[0]);
            if (type == null)
            {
                this.errors.Add($"Unparseable bound type {fields[0]} in {string.Join(' ', fields)}");
                return;
            }

            var boundType = type.Value;
            var boundsName = fields[1];
            var columnName = fields[2];

            // If bounds name is null, then use the last name. I think it is possible for the name to be
            // ommitted in subsequent lines once it is set.
            if (string.IsNullOrEmpty(boundsName))
            {
                boundsName = this.lastBoundsName;
            }
            else
            {
                this.lastBoundsName = boundsName;
            }

            if (boundType == MpsBound.Free || boundType == MpsBound.MI || boundType == MpsBound.PL)
            {
                model.AddBound(boundsName, columnName, boundType, double.NaN);
            }
            else
            {
                if (!double.TryParse(fields[3], out double coefficient))
                {
                    this.errors.Add($"Unparseable bound {fields[3]} in {string.Join(' ', fields)}");
                    return;
                }

                model.AddBound(boundsName, columnName, boundType, coefficient);
            }
        }

        /// <summary>
        /// Parses a range.
        /// </summary>
        /// <param name="fields">The fields in the line.</param>
        /// <param name="model">The linear programming model.</param>
        private void ParseRange(IList<string> fields, LPModel model)
        {
            if (fields.Count != 3 && fields.Count != 5)
            {
                this.errors.Add($"Invalid number of range fields {fields.Count} in {string.Join(' ', fields)}");
                return;
            }

            var rangesName = fields[0];
            var rowName = fields[1];

            // If ranges name is null, then use the last name. I think it is possible for the name to be
            // ommitted in subsequent lines once it is set.
            if (string.IsNullOrEmpty(rangesName))
            {
                rangesName = this.lastRangesName;
            }
            else
            {
                this.lastRangesName = rangesName;
            }

            if (double.TryParse(fields[2], out double coefficient))
            {
                model.AddRange(rangesName, rowName, coefficient);
            }
            else
            {
                this.errors.Add($"Unparseable range coefficient {fields[2]} in {string.Join(' ', fields)}");
            }

            if (fields.Count == 5)
            {
                rowName = fields[3];
                if (double.TryParse(fields[4], out coefficient))
                {
                    model.AddRange(rangesName, rowName, coefficient);
                }
                else
                {
                    this.errors.Add($"Unparseable range coefficient {fields[4]} in {string.Join(' ', fields)}");
                }
            }
        }
    }
}
