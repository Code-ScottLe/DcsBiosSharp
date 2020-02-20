using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DcsBiosSharp.Definition
{
    public class DcsBiosModuleInstrument : IModuleInstrument
    {
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
            get; private set;
        }

        public IReadOnlyList<IDcsBiosOutputDefinition> OutputDefinitions
        {
            get; private set;
        }

        public DcsBiosModuleInstrument(string category, string controlType, string description, string identifier, IEnumerable<IDcsBiosInputDefinition> inputDef, IEnumerable<IDcsBiosOutputDefinition> outputDefs)
        {
            Category = category;
            ControlType = controlType;
            Description = description;
            Identifier = identifier;
            InputDefinitions = inputDef is IReadOnlyList<IDcsBiosInputDefinition> listy ? listy : inputDef.ToList();
            OutputDefinitions = outputDefs is IReadOnlyList<IDcsBiosOutputDefinition> listy2 ? listy2 : outputDefs.ToList();

            foreach(var outputDef in OutputDefinitions)
            {
                outputDef.Instrument = this;
            }
        }
    }
}
