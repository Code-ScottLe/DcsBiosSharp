using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DcsBiosSharp.Client.Tests
{
    [TestClass]
    public class DcsBiosClientTests
    {
        [TestMethod]
        [Timeout(5000)]
        public async Task CurrentModuleTest_WithFirstAircraftSelected_UpdateFirstAircraftCorrectly()
        {
            // Arrange
            var buffer = new DcsBiosDataBuffer();
            var connection = GetMockedConnection();
            var manager = new ModuleDefinitionManager("./Assets/");
            await manager.RefreshModuleAsync();
            TaskCompletionSource<IModule> eventFiredSync = new TaskCompletionSource<IModule>();

            // Act
            var client = new DcsBiosClient(connection, buffer, manager);
            client.AircraftChanged += (s, e) =>
            {
                Debug.WriteLine(e?.Name ?? "Empty");
                eventFiredSync.SetResult(e);
            };
            await client.StartAsync();
            IModule newModule = await eventFiredSync.Task;

            // Assert
            Assert.AreEqual(expected: "FA-18C_hornet", newModule?.Name);
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
    }
}
