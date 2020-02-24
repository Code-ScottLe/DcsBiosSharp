using DcsBiosSharp.Definition.Inputs;

namespace DcsBiosSharp.Connection
{
    public class DcsBiosCommand : IDcsBiosCommand
    {
        public IDcsBiosInputDefinition InputDef
        {
            get; set;
        }

        public string CommandIdentifier
        {
            get; set;
        }

        public string Arguments
        {
            get; set;
        }

        public DcsBiosCommand(IDcsBiosInputDefinition definition, string arguments)
            : this (definition.Instrument.Identifier, arguments)
        {
            InputDef = definition;
        }

        public DcsBiosCommand(string commandIdentifier, string arguments)
        {
            CommandIdentifier = commandIdentifier;
            Arguments = arguments;
        }
    }
}
