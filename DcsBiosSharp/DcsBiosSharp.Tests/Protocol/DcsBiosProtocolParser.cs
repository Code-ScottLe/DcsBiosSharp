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
        [DataTestMethod]
        [DataRow("./Assets/dump.buffer")]
        [DataRow("./Assets/dump3.buff")]
        public void ParseBufferTest_WithValidBuffer_ReturnNonZeroUpdates(string bufferLocation)
        {
            // Arrange
            byte[] buffer = File.ReadAllBytes(bufferLocation);

            // Act

            var instance = new DcsBiosProtocolParser();
            IReadOnlyList<IDcsBiosExportData> output = instance.ParseBuffer(buffer);

            // Assert
            Assert.AreNotEqual(0, output.Count);
        }
    }
}
