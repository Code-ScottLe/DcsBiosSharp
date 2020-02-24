using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DcsBiosSharp.Client;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DcsBiosSharp.Tests
{
    [TestClass]
    public class E2ETests
    {
        [TestMethod]
        public async Task E2EDataExportScenarioTest_WithValidExportBuffer_UpdateAllOutputProperly()
        {
            // Arrange
            var buffer = new DcsBiosDataBuffer();
            var connection = GetMockedConnection();
            var manager = new ModuleDefinitionManager("./Assets/");

            string finalOption1Value = null;

            // Act
            var client = new DcsBiosClient(connection, buffer, manager);
            client.OutputsChanged += (s, e) =>
            {
                IDcsBiosOutputDefinition ufcOption1 = e.ChangedOutputs.FirstOrDefault(o => o.Instrument.Identifier == "UFC_OPTION_DISPLAY_1");
                if (ufcOption1 != null)
                {
                    finalOption1Value = ufcOption1.GetValueFromBuffer(e.Buffer.Buffer) as string;
                }
            };

            await client.StartAsync();

            // Assert
            // Wait for all of them to roll in.
            await Task.Delay(TimeSpan.FromSeconds(2));
            Assert.AreEqual(expected: "GRCV", actual: finalOption1Value);
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
