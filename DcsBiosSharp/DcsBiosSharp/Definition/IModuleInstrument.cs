using DcsBiosSharp.Definition.Inputs;
using DcsBiosSharp.Definition.Outputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Definition
{
    public interface IModuleInstrument
    {
        string Category { get; }

        string ControlType { get; }

        string Description { get;}

        string Identifier { get;  }

        IReadOnlyList<IDcsBiosInputDefinition> InputDefinitions { get; }

        IReadOnlyList<IDcsBiosOutputDefinition> OutputDefinitions { get; }
    }
}
