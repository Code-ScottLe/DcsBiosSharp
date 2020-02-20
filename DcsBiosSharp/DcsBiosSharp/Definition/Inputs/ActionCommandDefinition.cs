using DcsBiosSharp.Connection;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IModuleInstrument Instrument
        {
            get; set;
        }

        public ActionCommandDefinition(string actionArgs, string description = default(string))
        {
            Description = description;
            ActionArgs = actionArgs;
        }

        public IDcsBiosCommand CreateCommand(params object[] args)
        {
            var command = new DcsBiosCommand(DEFAULT_COMMAND_INTERFACE_NAME, ActionArgs, this);

            return command;
        }
    }
}
