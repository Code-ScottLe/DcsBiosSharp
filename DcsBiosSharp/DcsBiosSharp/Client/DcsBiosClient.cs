using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;

namespace DcsBiosSharp.Client
{
    public class DcsBiosClient
    {
        public IDcsBiosConnection Connection
        {
            get; private set;
        }

        public IDcsBiosDataBuffer DataBuffer
        {
            get; private set;
        }

        public IModuleDefinitionManager ModuleManager
        {
            get; private set;
        }

        public event EventHandler<DcsBioscClientOutputsChangedEventArgs> OutputsChanged;

        public DcsBiosClient()
            : this (new DcsBiosUdpConnection(), new DcsBiosDataBuffer(), new ModuleDefinitionManager())
        {
        }

        public DcsBiosClient(IDcsBiosConnection connection, IDcsBiosDataBuffer dataBuffer, IModuleDefinitionManager moduleManager)
        {
            Connection = connection;
            Connection.ExportDataReceived += OnConnectionReceivedExportData;

            DataBuffer = dataBuffer;
            DataBuffer.BufferUpdated += OnBufferUpdated;

            ModuleManager = moduleManager;
        }

        public async Task StartAsync()
        {
            await ModuleManager.RefreshModuleAsync();
            Connection.Start();
        }

        public Task SendCommandAsync(IDcsBiosInputDefinition inputDef, string args)
        {
            return SendCommandAsync(inputDef.CreateCommand(args));
        }

        public Task SendCommandAsync(IDcsBiosCommand command)
        {
            return Connection.SendCommandAsync(command);
        }

        public Task SendCommandAsync(string command, string args)
        {
            return SendCommandAsync(new DcsBiosCommand(command, args));
        }

        private void OnBufferUpdated(object sender, DcsBiosBufferUpdatedEventArgs e)
        {
            IEnumerable<IDcsBiosOutputDefinition> outputs = ModuleManager.Modules.SelectMany(m => m.Instruments)
                .SelectMany(i => i.OutputDefinitions).Where(o => e.StartIndex <= o.Address && o.Address + o.MaxSize <= e.EndIndex).Distinct();

            OutputsChanged?.Invoke(this, new DcsBioscClientOutputsChangedEventArgs(DataBuffer, outputs));
        }

        private void OnConnectionReceivedExportData(object sender, DcsBiosExportDataReceivedEventArgs e)
        {
            foreach (IDcsBiosExportData exportData in e.Data)
            {
                DataBuffer.HandleExportData(exportData);
            }
        }
    }
}
