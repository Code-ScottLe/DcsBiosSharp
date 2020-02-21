using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DcsBiosSharp.Tests
{
    [TestClass]
    public class E2ETests
    {
        [TestMethod]
        public void E2EDataExportScenarioTest_WithValidExportBuffer_UpdateAllOutputProperly()
        {
            // Arrange
            var buffer = new DcsBiosDataBuffer();
            var connection = GetMockedConnection();
            var sampleModule = GetSampleModule();

            connection.ExportDataReceived += buffer.OnExportDataReceived;

            // Act
            connection.Start();

            // Assert
            var scratchPad1 = sampleModule.Instruments.FirstOrDefault(i => i.Identifier == "UFC_OPTION_DISPLAY_1");
            Assert.AreEqual(expected: "GRCV", actual: scratchPad1.OutputDefinitions.FirstOrDefault().GetValueFromBuffer(buffer.Buffer));
        }

        private IDcsBiosConnection GetMockedConnection()
        {
            var mock = new Mock<IDcsBiosConnection>();
            mock.Setup(c => c.Start()).Callback(() =>
            {
                // buffer
                byte[] buffer = File.ReadAllBytes("./Assets/dump.buffer");
                var parser = new DcsBiosProtocolParser();
                IReadOnlyList<IDcsBiosExportData> data = parser.ParseBuffer(buffer);
                mock.Raise(m => m.ExportDataReceived += null, mock.Object, new DcsBiosExportDataReceivedEventArgs(data, DateTime.Now));
            });

            return mock.Object;
        }

        private IModule GetSampleModule()
        {
            string testModuleJsonPath = "./Assets/FA-18C_hornet.json";

            if (!File.Exists(testModuleJsonPath))
            {
                Assert.Inconclusive($"Test json is missing");
            }

            string moduleJson = File.ReadAllText(testModuleJsonPath);

            // Act
            var parser = new DcsBiosModuleDefinitionJsonParser();
            IModule module = parser.ParseModuleFromJson(Path.GetFileNameWithoutExtension(testModuleJsonPath), moduleJson);

            return module;
        }
    }
}
