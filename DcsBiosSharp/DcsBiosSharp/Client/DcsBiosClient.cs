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
    public class DcsBiosClient : INotifyPropertyChanged
    {
        private bool _isStarted;
        private IModule _currentModule;
        private ObservableCollection<DcsBiosOutput> _outputs;

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

        public IList<DcsBiosOutput> Outputs
        {
            get => _outputs;
        }

        public IModule CurrentModule
        {
            get => _currentModule;
            set
            {
                if(_currentModule != value)
                {
                    _currentModule = value;
                    OnPropertyChanged();
                }
            }
        }

        public event EventHandler<DcsBioscClientOutputsChangedEventArgs> OutputsChanged;
        public event EventHandler<IModule> AircraftChanged;
        public event PropertyChangedEventHandler PropertyChanged;

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

            _isStarted = false;

            _outputs = new ObservableCollection<DcsBiosOutput>();
        }

        public async Task StartAsync()
        {
            await ModuleManager.RefreshModuleAsync();

            // Get all of the outputs.
            foreach(IDcsBiosOutputDefinition defs in ModuleManager.Modules.SelectMany(m => m.Instruments).SelectMany(i => i.OutputDefinitions).Where(d => !Outputs.Any(o => o.Definition.Address == d.Address)))
            {
                DcsBiosOutput output = null;
                if (defs is IDcsBiosOutputDefinition<int> intDef)
                {
                    output = new DcsBiosOutput<int>(intDef, DataBuffer);
                }
                else if (defs is IDcsBiosOutputDefinition<string> stringDef)
                {
                    output = new DcsBiosOutput<string>(stringDef, DataBuffer);
                }

                if (output != null)
                {
                    Outputs.Add(output);
                }
            }

            var aircraftNameOutput = Outputs.FirstOrDefault(o => o.Definition.Instrument.Identifier == "_ACFT_NAME");
            if (aircraftNameOutput != null)
            {
                aircraftNameOutput.PropertyChanged += OnAircraftNameOutputChanged;
            }

            if (!_isStarted)
            {
                Connection.Start();
                _isStarted = true;
            }
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

        private void OnPropertyChanged([CallerMemberName]string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void OnAircraftNameOutputChanged(object sender, PropertyChangedEventArgs e)
        {
            // name change usually mean aircraft change
            if (sender is DcsBiosOutput<string> output && CurrentModule?.Name != output.Value)
            {
                CurrentModule = ModuleManager.GetModule(output.Value);
                AircraftChanged?.Invoke(this, CurrentModule);
            }
        }
    }
}
