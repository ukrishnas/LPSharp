// --------------------------------------------------------------------------------------------------------------------
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
    /// Represents a reader of mathematical programming system MPS format files.
    /// https://www.cenapad.unicamp.br/parque/manuais/OSL/oslweb/features/featur11.htm explains
    /// the format. This reader reads fixed and free format files, supports the RANGES optional
    /// section, does not understand integer data, and non-LP sections.
    /// </summary>
    public class MpsReader
    {
        /// <summary>
        /// The field length of the number fields is 12 characters per the specification.
        /// But MPS files use the maximum possible 14 characters.
        /// </summary>
        private const int NumberFieldLength = 14;

        /// <summary>
        /// All name fields are always eight characters wide.
        /// </summary>
        private const int NameFieldLength = 8;

        /// <summary>
        /// Fixed MPS format field position and length for section headers.
        /// </summary>
        private readonly IList<Tuple<int, int>> sectionFieldPositions = new List<Tuple<int, int>>
        {
            new(1, 12),
            new(15, NameFieldLength),
        };

        /// <summary>
        /// Fixed MPS format field position and length for rows and bounds sections.
        /// </summary>
        private readonly IList<Tuple<int, int>> rowFieldPositions = new List<Tuple<int, int>>
        {
            new(2, 2),
            new(5, NameFieldLength),
            new(15, NameFieldLength),
            new(25, NumberFieldLength),
        };

        /// <summary>
        /// Fixed MPS format field position and length for columns, RHS, and ranges sections.
        /// </summary>
        private readonly IList<Tuple<int, int>> rolumnFieldPositions = new List<Tuple<int, int>>
        {
            new(5, NameFieldLength),
            new(15, NameFieldLength),
            new(25, NumberFieldLength),
            new(40, NameFieldLength),
            new(50, NumberFieldLength),
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
        /// The last bounds name.
        /// </summary>
        private string lastBoundsName = "Default";

        /// <summary>
        /// The last ranges name.
        /// </summary>
        private string lastRangesName = "Default";

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
        /// Reads a mathematical programming system (MPS) file.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="mpsFormat">The MPS file format.</param>
        /// <returns>The linear programming model.</returns>
        public LPModel Read(string filename, MpsFormat mpsFormat = MpsFormat.Fixed)
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
                    using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
                    this.ParseMps(gzipStream, model, mpsFormat);
                }
                else
                {
                    this.ParseMps(stream, model, mpsFormat);
                }
            }

            if (!model.IsValid())
            {
                this.errors.Add($"Model failed validation checks for construction and bounds");
            }

            return model;
        }

        /// <summary>
        /// Parses an MPS stream.
        /// </summary>
        /// <param name="stream">The MPS stream.</param>
        /// <param name="model">The linear programming model.</param>
        /// <param name="mpsFormat">The file format.</param>
        private void ParseMps(Stream stream, LPModel model, MpsFormat mpsFormat)
        {
            string line;
            MpsSection? section = null;

            var reader = new StreamReader(stream);
            while ((line = reader.ReadLine()) != null)
            {
                var fields = this.ParseLine(line, section, mpsFormat, out bool sectionLine);
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
                        if (fields.Count > 1)
                        {
                            model.Name = fields[1];
                        }
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

            model.SetObjective();
        }

        /// <summary>
        /// Parses a line into fields.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="section">The section.</param>
        /// <param name="mpsFormat">The file format.</param>
        /// <param name="sectionLine">If true, parsed a section header (output parameter).</param>
        /// <returns>The list of fields.</returns>
        private IList<string> ParseLine(
            string line,
            MpsSection? section,
            MpsFormat mpsFormat,
            out bool sectionLine)
        {
            // Return null for comment line.
            if (string.IsNullOrEmpty(line) || line[0] == '*')
            {
                sectionLine = false;
                return null;
            }

            sectionLine = line[0] != ' ';
            var fields = new List<string>();

            // It is okay to trim the end of the line so that whitespace at the end
            // of the line does not become columns.
            line = line.TrimEnd();

            IList<Tuple<int, int>> positions;

            if (mpsFormat == MpsFormat.Fixed)
            {
                if (sectionLine || section == null)
                {
                    positions = this.sectionFieldPositions;
                }
                else if (section == MpsSection.Rows || section == MpsSection.Bounds)
                {
                    positions = this.rowFieldPositions;
                }
                else
                {
                    positions = this.rolumnFieldPositions;
                }
            }
            else
            {
                positions = this.DetectFieldPositions(line);
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
        /// Detects field positions in a free format line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The field positions.</returns>
        private IList<Tuple<int, int>> DetectFieldPositions(string line)
        {
            var positions = new List<Tuple<int, int>>();

            int? fieldStart = null;
            int fieldLength = 0;

            for (int i = 1; i <= line.Length + 1; i++)
            {
                // Field positions are indexed from 1. Hence subtract before indexing.
                // Also the iterator goes past line length so that the last field can be stored.
                var c = (i == line.Length + 1) ? ' ' : line[i - 1];
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    if (fieldStart != null)
                    {
                        // Terminate field upon encountering a white space.
                        positions.Add(new(fieldStart.Value, fieldLength));
                        fieldStart = null;
                        fieldLength = 0;
                    }
                }
                else
                {
                    if (fieldStart == null)
                    {
                        // Start field upon encountering a non-whitespace character.
                        fieldStart = i;
                        fieldLength = 1;
                    }
                    else
                    {
                        // Increment current field upon encountering a non-whitespace character.
                        fieldLength++;
                    }
                }
            }

            return positions;
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
                this.errors.Add($"Cannot parse row type {fields[0]}");
                return;
            }

            // When a row type is read, create a row in the A matrix along with the row in
            // the row type vector. Otherwise, an empty row will not get constraints and
            // cause solvers to behave differently while giving the same objective result.
            var name = fields[1];
            model.RowTypes[name] = type.Value;
            model.A[name, null] = 0;
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
                model.A[rowName, columnName] = value;
            }
            else
            {
                this.errors.Add($"Cannot parse column coefficient {fields[2]} in {string.Join(' ', fields)}");
            }

            if (fields.Count == 5)
            {
                rowName = fields[3];
                if (double.TryParse(fields[4], out value))
                {
                    model.A[rowName, columnName] = value;
                }
                else
                {
                    this.errors.Add($"Cannot parse column coefficient {fields[4]} in {string.Join(' ', fields)}");
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
            // omitted in subsequent lines once it is set.
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
                model.B[rhsName, rowName] = coefficient;
            }
            else
            {
                this.errors.Add($"Cannot parse RHS coefficient {fields[2]} in {string.Join(' ', fields)}");
            }

            if (fields.Count == 5)
            {
                rowName = fields[3];
                if (double.TryParse(fields[4], out coefficient))
                {
                    model.B[rhsName, rowName] = coefficient;
                }
                else
                {
                    this.errors.Add($"Cannot parse RHS coefficient {fields[4]} in {string.Join(' ', fields)}");
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
                this.errors.Add($"Cannot parse bound type {fields[0]} in {string.Join(' ', fields)}");
                return;
            }

            var boundType = type.Value;
            var boundsName = fields[1];
            var columnName = fields[2];

            // If bounds name is null, then use the last name. I think it is possible for the name to be
            // omitted in subsequent lines once it is set.
            if (string.IsNullOrEmpty(boundsName))
            {
                boundsName = this.lastBoundsName;
            }
            else
            {
                this.lastBoundsName = boundsName;
            }

            if (boundType == MpsBound.Free || boundType == MpsBound.MinusInfinity || boundType == MpsBound.PlusInfinity)
            {
                model.SetBound(boundsName, columnName, boundType, default /* ignored */);
            }
            else
            {
                if (!double.TryParse(fields[3], out double coefficient))
                {
                    this.errors.Add($"Cannot parse bound {fields[3]} in {string.Join(' ', fields)}");
                    return;
                }

                model.SetBound(boundsName, columnName, boundType, coefficient);
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
            // omitted in subsequent lines once it is set.
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
                model.R[rangesName, rowName] = coefficient;
            }
            else
            {
                this.errors.Add($"Cannot parse range coefficient {fields[2]} in {string.Join(' ', fields)}");
            }

            if (fields.Count == 5)
            {
                rowName = fields[3];
                if (double.TryParse(fields[4], out coefficient))
                {
                    model.R[rangesName, rowName] = coefficient;
                }
                else
                {
                    this.errors.Add($"Cannot parse range coefficient {fields[4]} in {string.Join(' ', fields)}");
                }
            }
        }
    }
}
