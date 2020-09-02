using System.Collections.Generic;
using System.Linq;
using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;

namespace DcsBiosSharp.Definition
{
    public class DcsBiosModuleInstrumentDefinition : IModuleInstrumentDefinition
    {
        private List<IDcsBiosOutputDefinition> _outputDefinitions;
        private List<IDcsBiosInputDefinition> _inputDefinitions;

        public string Category
        {
            get; private set;
        }

        public string ControlType
        {
            get; private set;
        }

        public string Description
        {
            get; private set;
        }

        public string Identifier
        {
            get; private set;
        }

        public IReadOnlyList<IDcsBiosInputDefinition> InputDefinitions
        {
            get => _inputDefinitions;
        }

        public IReadOnlyList<IDcsBiosOutputDefinition> OutputDefinitions
        {
            get => _outputDefinitions;
        }

        public DcsBiosModuleInstrumentDefinition(string category, string controlType, string description, string identifier)
        {
            Category = category;
            ControlType = controlType;
            Description = description;
            Identifier = identifier;
            _inputDefinitions = new List<IDcsBiosInputDefinition>();
            _outputDefinitions = new List<IDcsBiosOutputDefinition>();
        }

        public void AddInput(IDcsBiosInputDefinition inputDef)
        {
            _inputDefinitions.Add(inputDef);
        }

        public void AddInputs(IEnumerable<IDcsBiosInputDefinition> inputDefs)
        {
            _inputDefinitions.AddRange(inputDefs);
        }

        public void AddOutput(IDcsBiosOutputDefinition outputDef)
        {
            _outputDefinitions.Add(outputDef);
        }

        public void AddOutputs(IEnumerable<IDcsBiosOutputDefinition> outputDefs)
        {
            _outputDefinitions.AddRange(outputDefs);
        }
    }
}
