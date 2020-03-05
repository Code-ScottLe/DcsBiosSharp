using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public void ParseBufferTest_WithValidBufferArray_ReturnNonZeroUpdates(string bufferLocation)
        {
            // Arrange
            byte[] buffer = File.ReadAllBytes(bufferLocation);

            // Act

            var instance = new DcsBiosProtocolParser();
            IReadOnlyList<IDcsBiosExportData> output = instance.ParseBuffer(buffer);

            // Assert
            Assert.AreNotEqual(0, output.Count);
        }

        [TestMethod]
        public void ParserBufferTest_WithValidBufferArray_ReturnExpectedBufferUpdates()
        {
            // Arrange
            byte[] buffer = File.ReadAllBytes("./Assets/dump.buffer");

            // Act
            var instance = new DcsBiosProtocolParser();
            IReadOnlyList<IDcsBiosExportData> output = instance.ParseBuffer(buffer);

            // Assert

            // First.
            IDcsBiosExportData first = output.First();
            Assert.AreEqual(expected: 0, first.Address);
            Assert.AreEqual(expected: 0x18, actual: first.Data.Count);

            // FA-18C_hornet
            byte[] firstBuffer = new byte[0x18] { 0x46, 0x41, 0x2d, 0x31, 0x38, 0x43, 0x5f, 0x68, 0x6f, 0x72, 0x6e, 0x65, 0x74, 0x00, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            for (int i = 0; i < firstBuffer.Length; i++)
            {
                Assert.AreEqual(expected: firstBuffer[i], actual: first.Data[i]);
            }

            // second.
            IDcsBiosExportData second = output[1];
            Assert.AreEqual(expected: 0x7400, actual: second.Address);
            Assert.AreEqual(expected: 0x148, actual: second.Data.Count);

            // Third.
            IDcsBiosExportData third = output[2];
            Assert.AreEqual(expected: 0x400, actual: third.Address);
            Assert.AreEqual(expected: 0x24, actual: third.Data.Count);
            byte[] thridBuffer = new byte[0x24] { 0x4e, 0x65, 0x77, 0x20, 0x63, 0x61, 0x6c, 0x6c, 0x73, 0x69, 0x67, 0x6e, 0x00, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xd9, 0x01, 0xbb, 0x7a, 0x77, 0x03, 0x6b, 0x5d, 0xd0, 0x59, 0x2f, 0x77 };

            for (int i = 0; i < thridBuffer.Length; i++)
            {
                Assert.AreEqual(expected: thridBuffer[i], actual: third.Data[i]);
            }

            // Fifth
            IDcsBiosExportData fifth = output[4];
            Assert.AreEqual(expected: 0x4, actual: fifth.Address);
            Assert.AreEqual(expected: 0x2, actual: fifth.Data.Count);
            byte[] fifthBuffer = new byte[2] { 0x38, 0x43 };

            for (int i = 0; i < fifthBuffer.Length; i++)
            {
                Assert.AreEqual(expected: fifthBuffer[i], actual: fifth.Data[i]);
            }

            // Last.
            IDcsBiosExportData last = output.Last();
            Assert.AreEqual(expected: 0xfffe, actual: last.Address);
            Assert.AreEqual(expected: 0x2, actual: last.Data.Count);
            byte[] lastBuffer = new byte[2] { 0xa8, 0x00 };

            for (int i = 0; i < lastBuffer.Length; i++)
            {
                Assert.AreEqual(expected: lastBuffer[i], actual: last.Data[i]);
            }
        }

        [TestMethod]
        public void ParserBufferTest_WithValidBufferList_ReturnExpectedBufferUpdates()
        {
            // Arrange
            byte[] buffer = File.ReadAllBytes("./Assets/dump.buffer");

            // Act
            var instance = new DcsBiosProtocolParser();
            IReadOnlyList<IDcsBiosExportData> output = instance.ParseBuffer(buffer.ToList());

            // Assert

            // First.
            IDcsBiosExportData first = output.First();
            Assert.AreEqual(expected: 0, first.Address);
            Assert.AreEqual(expected: 0x18, actual: first.Data.Count);

            // FA-18C_hornet
            byte[] firstBuffer = new byte[0x18] { 0x46, 0x41, 0x2d, 0x31, 0x38, 0x43, 0x5f, 0x68, 0x6f, 0x72, 0x6e, 0x65, 0x74, 0x00, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            for (int i = 0; i < firstBuffer.Length; i++)
            {
                Assert.AreEqual(expected: firstBuffer[i], actual: first.Data[i]);
            }

            // second.
            IDcsBiosExportData second = output[1];
            Assert.AreEqual(expected: 0x7400, actual: second.Address);
            Assert.AreEqual(expected: 0x148, actual: second.Data.Count);

            // Third.
            IDcsBiosExportData third = output[2];
            Assert.AreEqual(expected: 0x400, actual: third.Address);
            Assert.AreEqual(expected: 0x24, actual: third.Data.Count);
            byte[] thridBuffer = new byte[0x24] { 0x4e, 0x65, 0x77, 0x20, 0x63, 0x61, 0x6c, 0x6c, 0x73, 0x69, 0x67, 0x6e, 0x00, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xd9, 0x01, 0xbb, 0x7a, 0x77, 0x03, 0x6b, 0x5d, 0xd0, 0x59, 0x2f, 0x77 };

            for (int i = 0; i < thridBuffer.Length; i++)
            {
                Assert.AreEqual(expected: thridBuffer[i], actual: third.Data[i]);
            }

            // Fifth
            IDcsBiosExportData fifth = output[4];
            Assert.AreEqual(expected: 0x4, actual: fifth.Address);
            Assert.AreEqual(expected: 0x2, actual: fifth.Data.Count);
            byte[] fifthBuffer = new byte[2] { 0x38, 0x43 };

            for (int i = 0; i < fifthBuffer.Length; i++)
            {
                Assert.AreEqual(expected: fifthBuffer[i], actual: fifth.Data[i]);
            }

            // Last.
            IDcsBiosExportData last = output.Last();
            Assert.AreEqual(expected: 0xfffe, actual: last.Address);
            Assert.AreEqual(expected: 0x2, actual: last.Data.Count);
            byte[] lastBuffer = new byte[2] { 0xa8, 0x00 };

            for (int i = 0; i < lastBuffer.Length; i++)
            {
                Assert.AreEqual(expected: lastBuffer[i], actual: last.Data[i]);
            }
        }
    }
}
