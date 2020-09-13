using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;

namespace DcsBiosSharp.Client
{
    public class DcsBiosClient : BindableBase, IDisposable
    {
        public const string DEFAULT_METADATA_START_MODULE_NAME = "MetadataStart";
        public const string DEFAULT_METADATA_END_MOdULE_NAME = "MetadataEnd";

        public const string DEFAULT_COMMON_DATA_MODULE_NAME = "CommonData";

        private DcsBiosOutput<string> _aircraftNameOutput;

        protected bool disposed = false;

        public IDcsBiosConnection DcsConnection
        {
            get; private set;
        }

        public IDcsBiosDataBuffer Buffer
        {
            get; private set;
        }

        public IModuleDefinitionManager DefManager
        {
            get; private set;
        }

        public DcsBiosClient(IModuleDefinitionManager moduleDefinitionManager)
            : this (new DcsBiosUdpConnection(), moduleDefinitionManager)
        {
        }

        public DcsBiosClient(IDcsBiosConnection connection, IModuleDefinitionManager moduleDefinitionManager)
            : this (connection, new DcsBiosDataBuffer(), moduleDefinitionManager)
        {
        }

        public DcsBiosClient(IDcsBiosConnection connection, IDcsBiosDataBuffer buffer, IModuleDefinitionManager moduleDefinitionManager)
        {
            DcsConnection = connection;
            Buffer = buffer;
            DefManager = moduleDefinitionManager;

            // Subscribe to the metadata for the aircraft change.
            IModuleDefinition metadata = DefManager.Modules[DEFAULT_METADATA_START_MODULE_NAME];
            var aircraftNameOutputDef =  metadata.Instruments.First(i => i.Identifier == "_ACFT_NAME").OutputDefinitions.Single() as IDcsBiosOutputDefinition<string>;

            _aircraftNameOutput = new DcsBiosOutput<string>(aircraftNameOutputDef, Buffer);
            _aircraftNameOutput.PropertyChanged += OnAircraftNameChanged;
        }

        public async Task ConnectAsync()
        {

        }

        private void OnAircraftNameChanged(object sender, PropertyChangedEventArgs e)
        {
            // Look up new aircrafts?
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                _aircraftNameOutput.Dispose();
            }

            disposed = true;
        }
    }
}
