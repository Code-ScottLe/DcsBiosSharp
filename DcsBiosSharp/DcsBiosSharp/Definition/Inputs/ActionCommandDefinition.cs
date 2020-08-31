using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Definition.Inputs
{
    public class ActionCommandDefinition : IDcsBiosInputDefinition
    {
        public const string DEFAULT_COMMAND_INTERFACE_NAME = "action";

        public string Name => DEFAULT_COMMAND_INTERFACE_NAME;

        public string Description
        {
            get; private set;
        }

        public bool HasArgs => false;

        public string ActionArgs
        {
            get; private set;
        }

        public IModuleInstrumentDefinition Instrument
        {
            get; set;
        }

        public ActionCommandDefinition(IModuleInstrumentDefinition instrument, string actionArgs, string description = default(string))
        {
            Instrument = instrument;
            Description = description;
            ActionArgs = actionArgs;
        }

        public IDcsBiosCommand CreateCommand(params object[] args)
        {
            var command = new DcsBiosCommand(this, ActionArgs);

            return command;
        }
    }
}
