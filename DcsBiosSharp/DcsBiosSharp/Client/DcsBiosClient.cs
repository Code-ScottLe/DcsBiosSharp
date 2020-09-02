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
    public class DcsBiosClient : BindableBase
    {
        private IModuleDefinition _module;

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

        public IModuleDefinition Module
        {
            get => _module;
            set => Set(ref _module, value);
        }

        public DcsBiosClient(IDcsBiosConnection connection, IDcsBiosDataBuffer buffer, IModuleDefinitionManager moduleDefinitionManager)
        {
            DcsConnection = connection;
            Buffer = buffer;
            DefManager = moduleDefinitionManager;

            // Subscribe to the metadata for the aircraft change.
            IModuleDefinition metadata = DefManager.Modules[ModuleDefinitionManager.DEFAULT_METADATA_MODULE_NAME];
            var aircraftNameOutputDef =  metadata.Instruments.First(i => i.Identifier == "_ACFT_NAME").OutputDefinitions.Single() as IDcsBiosOutputDefinition<string>;
        }

        public static async Task<DcsBiosClient> CreateAsync()
        {
            var connection = new DcsBiosUdpConnection();
            var buffer = new DcsBiosDataBuffer(connection);
            var moduleManager = await ModuleDefinitionManager.CreateAsync();

            var instance = new DcsBiosClient(connection, buffer, moduleManager);

            return instance;
        }
    }
}
