using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
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

        public DcsBiosClient(IDcsBiosConnection connection, IDcsBiosDataBuffer dataBuffer, IModuleDefinitionManager moduleManager)
        {
            Connection = connection;
            Connection.ExportDataReceived += OnConnectionReceivedExportData;

            DataBuffer = dataBuffer;
            DataBuffer.BufferUpdated += OnBufferUpdated;

            ModuleManager = moduleManager;
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
