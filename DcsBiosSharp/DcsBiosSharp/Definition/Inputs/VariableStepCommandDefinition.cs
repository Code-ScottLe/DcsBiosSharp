using System;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Definition.Inputs
{
    public class VariableStepCommandDefinition : IDcsBiosInputDefinition
    {
        public const string DEFAULT_COMMAND_INTERFACE_NAME = "variable_step";

        public string Name => DEFAULT_COMMAND_INTERFACE_NAME;

        public double MaxValue
        {
            get; set;
        }

        public double SuggestedSteps
        {
            get; set;
        }

        public string Description
        {
            get; private set;
        }

        public bool HasArgs => true;

        public IModuleInstrument Instrument
        {
            get; set;
        }

        public VariableStepCommandDefinition(IModuleInstrument moduleInstrument, double maxValue, double suggestedSteps, string description = default(string))
        {
            MaxValue = maxValue;
            SuggestedSteps = suggestedSteps;
            Description = description;
        }

        public IDcsBiosCommand CreateCommand(params object[] args)
        {
            string commandArg;

            if (args.Length != 1)
            {
                throw new ArgumentOutOfRangeException("SetState expected only one argument");
            }
            else if (args[0] is string cmd)
            {
                switch(cmd)
                {
                    case "INC":
                        commandArg = $"+{SuggestedSteps}";
                        break;
                    case "DEC":
                        commandArg = $"-{SuggestedSteps}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"VariableStep was given a string command that it didn't understand: {cmd}. Only expect INC/DEC");
                }
            }
            else if (!(args[0] is double value))
            {
                throw new ArgumentException($"VariableStep was given args with incompatible type {args.GetType().Name}");
            }
            else if (value > MaxValue)
            {
                throw new ArgumentOutOfRangeException($"args is exceed the max value based on definition.");
            }
            else
            {
                commandArg = value < 0 ? $"-{value}" : $"+{value}"; 
            }

            var instance = new DcsBiosCommand(this, commandArg);
            return instance;
        }

    }
}
