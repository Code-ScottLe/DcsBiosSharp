using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DcsBiosSharp.Connection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DcsBiosSharp.Protocol.Tests
{
    [TestClass]
    public class DcsBiosProtocolParserTest
    {
        [TestMethod]
        public void ParseBufferTest_WithValidBuffer_ReturnNonZeroUpdates()
        {
            // Arrange
            FileStream testBufferStream = File.OpenRead("./Assets/dump.buffer");
            BinaryReader binary = new BinaryReader(testBufferStream);

            byte[] buffer = binary.ReadBytes((int)testBufferStream.Length);

            binary.Dispose();
            testBufferStream.Dispose();

            // Act

            var instance = new DcsBiosProtocolParser();
            IReadOnlyList<IDcsBiosExportData> output = instance.ParseBuffer(buffer);

            // Assert
            Assert.AreNotEqual(0, output.Count);
        }
    }
}
