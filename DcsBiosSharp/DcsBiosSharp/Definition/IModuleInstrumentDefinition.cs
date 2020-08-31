using System.Collections.Generic;
using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;

namespace DcsBiosSharp.Definition
{
    public interface IModuleInstrumentDefinition
    {
        string Category { get; }

        string ControlType { get; }

        string Description { get; }

        string Identifier { get; }

        IReadOnlyList<IDcsBiosInputDefinition> InputDefinitions { get; }

        IReadOnlyList<IDcsBiosOutputDefinition> OutputDefinitions { get; }
    }
}
