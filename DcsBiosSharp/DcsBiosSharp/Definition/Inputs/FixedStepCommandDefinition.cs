using System;
using System.Linq;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Definition.Inputs
{
    public class FixedStepCommandDefinition : IDcsBiosInputDefinition
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

        public FixedStepCommandDefinition(IModuleInstrument instrument)
            : this(instrument, DEFAULT_COMMAND_DESCRIPTION)
        {
        }

        public FixedStepCommandDefinition(IModuleInstrument instrument, string description)
        {
            Instrument = instrument;
            Description = description;
        }

        public IDcsBiosCommand CreateCommand(params object[] args)
        {
            if (!args.Any())
            {
                throw new ArgumentNullException(paramName: nameof(args));
            }

            switch (args.First())
            {
                case "INC":
                    return new DcsBiosCommand(this, "INC");
                case "DEC":
                    return new DcsBiosCommand(this, "DEC");
                default:
                    throw new ArgumentException(message: "Fixed Step accepts only INC or DEC as input");
            }
        }
    }
}
