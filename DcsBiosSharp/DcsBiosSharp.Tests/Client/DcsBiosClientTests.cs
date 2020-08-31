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
        //[TestMethod]
        //public async Task CurrentModuleTest_WithFirstAircraftSelected_UpdateFirstAircraftCorrectly()
        //{
        //    // Arrange
        //    var buffer = new DcsBiosDataBuffer();
        //    TaskCompletionSource<bool> _connectionDoneSending = new TaskCompletionSource<bool>();
        //    var connection = GetMockedConnection(_connectionDoneSending);
        //    var manager = new ModuleDefinitionManager("./Assets/");
        //    await manager.RefreshModulesAsync();
        //    IModuleDefinition module = null;

        //    // Act
        //    var client = new DcsBiosClient(connection, buffer, manager);
        //    client.AircraftChanged += (s, e) =>
        //    {
        //        Debug.WriteLine(e?.Name ?? "Empty");
        //        module = e;
        //    };
        //    await client.StartAsync();

        //    await _connectionDoneSending.Task;

        //    // Assert
        //    Assert.AreEqual(expected: "FA-18C_hornet", module?.Name);
        //}

        private IDcsBiosConnection GetMockedConnection(TaskCompletionSource<bool> signalingTask = null)
        {
            var mock = new Mock<IDcsBiosConnection>();
            mock.Setup(c => c.Start()).Callback(() =>
            {
                // buffer
                byte[] buffer = File.ReadAllBytes("./Assets/dump.buffer");
                var parser = new DcsBiosProtocolParser();
                IReadOnlyList<IDcsBiosExportData> data = parser.ParseBuffer(buffer);
                mock.Raise(m => m.ExportDataReceived += null, mock.Object, new DcsBiosExportDataReceivedEventArgs(data, DateTime.Now));

                if (signalingTask != null)
                {
                    signalingTask.SetResult(true);
                }
            });

            return mock.Object;
        }
    }
}
