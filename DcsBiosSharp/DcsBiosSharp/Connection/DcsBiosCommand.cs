using DcsBiosSharp.Definition.Inputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Connection
{
    public class DcsBiosCommand : IDcsBiosCommand
    {
        public IDcsBiosInputDefinition InputDef
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Arguments
        {
            get; set;
        }

        public DcsBiosCommand(string name, string arguments = default, IDcsBiosInputDefinition definition = default)
        {
            Name = name;
            Arguments = arguments;
            InputDef = definition;
        }
    }
}
