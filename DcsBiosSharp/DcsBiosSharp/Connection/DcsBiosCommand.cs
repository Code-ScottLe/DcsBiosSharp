using DcsBiosSharp.Definition.Inputs;

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
