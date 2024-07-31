// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MpsReaderTest.cs">
// Copyright (c) 2024 Umesh Krishnaswamy.
// Licensed under the MIT License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LPSharp.LPDriverTest
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using LPSharp.LPDriver.Contract;
    using LPSharp.LPDriver.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="MpsReader"/> class.
    /// </summary>
    [TestClass]
    public class MpsReaderTest
    {
        /// <summary>
        /// Tests MPS read operation with example model files.
        /// </summary>
        [TestMethod]
        public void MpsReadExampleModelTest()
        {
            var reader = new MpsReader();
            foreach (var test in TestUtil.ExampleModels)
            {
                var filename = $"TestData\\{test.Item1}";
                Assert.IsTrue(File.Exists(filename), $"{filename} not present");
                reader.Read(filename);
                Assert.AreEqual(0, reader.Errors.Count, $"Read errors {filename}");
            }
        }

        /// <summary>
        /// Tests MPS read operation with GZip model files.
        /// </summary>
        [TestMethod]
        public void MpsReadGzipModelTest()
        {
            var reader = new MpsReader();
            foreach (var test in TestUtil.ExampleModels)
            {
                var filename = $"TestData\\{test.Item1}";
                var compressedFilename = CompressFile(filename);
                Assert.IsNotNull(compressedFilename, $"Unable to compress {filename}");

                reader.Read(compressedFilename);
                Assert.AreEqual(0, reader.Errors.Count, $"Read errors {filename} {compressedFilename}");
            }
        }

        /// <summary>
        /// Tests MPS read operation with free format model files.
        /// </summary>
        [TestMethod]
        public void MpsReadFreeFormatModelTest()
        {
            var reader = new MpsReader();
            foreach (var test in TestUtil.FreeFormatModels)
            {
                var filename = $"TestData\\{test.Item1}";
                Assert.IsTrue(File.Exists(filename), $"{filename} not present");
                reader.Read(filename, MpsFormat.Free);
                Assert.AreEqual(0, reader.Errors.Count, $"Read errors {filename}");
            }
        }

        /// <summary>
        /// Compresses a model file.
        /// </summary>
        /// <param name="filename">The model file name.</param>
        /// <returns>The compressed model file name.</returns>
        private static string CompressFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var content = File.ReadAllBytes(filename);

            var compressedFilename = Guid.NewGuid().ToString() + ".gz";
            using var stream = new FileStream(compressedFilename, FileMode.Create);
            using var gzipStream = new GZipStream(stream, CompressionMode.Compress);
            gzipStream.Write(content);

            return compressedFilename;
        }
    }
}
