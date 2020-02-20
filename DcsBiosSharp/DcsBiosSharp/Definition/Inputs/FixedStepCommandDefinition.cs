using DcsBiosSharp.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Definition.Inputs
{
    public class FixedStepCommandDefinition : IDcsBiosInputDefinition , IDcsBiosCommand
    {
        private const string DEFAULT_COMMAND_DESCRIPTION = "switch to previous or next state";

        public const string DEFAULT_COMMAND_INTERFACE_NAME = "fixed_step";

        public string Name
        {
            get => DEFAULT_COMMAND_INTERFACE_NAME;
        }

        public string Description
        {
            get; set;
        }

        public bool HasArgs => false;

        public string Arguments => string.Empty;

        public IModuleInstrument Instrument
        {
            get; set;
        }

        public FixedStepCommandDefinition()
            : this(DEFAULT_COMMAND_DESCRIPTION)
        {
        }

        public FixedStepCommandDefinition(string description)
        {
            Description = description;
        }

        public IDcsBiosCommand CreateCommand(params object[] args)
        {
            return this;
        }
    }
}
