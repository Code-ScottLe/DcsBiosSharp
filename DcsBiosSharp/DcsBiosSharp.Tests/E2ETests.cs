using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    //    [TestMethod]
    //    public async Task E2EDataExportScenarioTest_WithValidExportBuffer_UpdateAllOutputProperly()
    //    {
    //        // Arrange
    //        TaskCompletionSource<bool> signalTask = new TaskCompletionSource<bool>();
    //        var buffer = new DcsBiosDataBuffer();
    //        var connection = GetMockedConnection(signalTask);
    //        var manager = new ModuleDefinitionManager("./Assets/");
    //        await manager.RefreshModulesAsync();

    //        // Act
    //        var client = new DcsBiosClient(connection, buffer, manager);
    //        await client.StartAsync();

    //        DcsBiosOutput<string> output = client.Outputs.FirstOrDefault(o => o.Definition.Instrument.Identifier == "UFC_OPTION_DISPLAY_1") as DcsBiosOutput<string>;

    //        // Assert
    //        // Wait for all of them to roll in.
    //        await signalTask.Task;
    //        Assert.AreEqual(expected: "GRCV", actual: output.Value);
    //    }

    //    [TestMethod]
    //    public async Task E2EDataExportScenarioTest_WithValidExportDuplicatedBuffer_OnlyNotifyChangedOnce()
    //    {
    //        // Arrange
    //        TaskCompletionSource<bool> signalTask = new TaskCompletionSource<bool>();
    //        var buffer = new DcsBiosDataBuffer();
    //        var connection = GetMockedConnectionWithDuplicates(signalTask);
    //        var manager = new ModuleDefinitionManager("./Assets/");
    //        await manager.RefreshModulesAsync();

    //        // This was due to the initial update will also be counted as one update.
    //        int propertyChangedCounter = -1;

    //        // Act
    //        var client = new DcsBiosClient(connection, buffer, manager);
    //        await client.StartAsync();

    //        DcsBiosOutput<string> output = client.Outputs.FirstOrDefault(o => o.Definition.Instrument.Identifier == "UFC_OPTION_DISPLAY_1") as DcsBiosOutput<string>;
    //        output.PropertyChanged += (s, e) =>
    //        {
    //            propertyChangedCounter++;
    //        };

    //        // Assert
    //        // Wait for all of them to roll in.
    //        await signalTask.Task;
    //        Assert.AreEqual(expected: 1, actual: propertyChangedCounter);
    //    }

        private IDcsBiosConnection GetMockedConnection(TaskCompletionSource<bool> signalTask = null)
        {
            var mock = new Mock<IDcsBiosConnection>();
            mock.Setup(c => c.Start()).Callback(async () =>
            {
                // buffer
                byte[] buffer = File.ReadAllBytes("./Assets/dump.buffer");
                var parser = new DcsBiosProtocolParser();
                IReadOnlyList<IDcsBiosExportData> data = parser.ParseBuffer(buffer);
                mock.Raise(m => m.ExportDataReceived += null, mock.Object, new DcsBiosExportDataReceivedEventArgs(data, DateTime.Now));

                await Task.Delay(2000);
                if (signalTask != null)
                {
                    signalTask.SetResult(true);
                }
            });

            return mock.Object;
        }

        private IDcsBiosConnection GetMockedConnectionWithDuplicates(TaskCompletionSource<bool> signalTask = null)
        {
            var mock = new Mock<IDcsBiosConnection>();
            mock.Setup(c => c.Start()).Callback(async () =>
            {
                // buffer
                await Task.Delay(2000);
                byte[] buffer = File.ReadAllBytes("./Assets/dump3.buff");
                var parser = new DcsBiosProtocolParser();
                IReadOnlyList<IDcsBiosExportData> data = parser.ParseBuffer(buffer);
                mock.Raise(m => m.ExportDataReceived += null, mock.Object, new DcsBiosExportDataReceivedEventArgs(data, DateTime.Now));

                await Task.Delay(2000);
                if (signalTask != null)
                {
                    signalTask.SetResult(true);
                }
            });

            return mock.Object;
        }
    }
}
