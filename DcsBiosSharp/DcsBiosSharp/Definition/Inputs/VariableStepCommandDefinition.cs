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
            throw new NotImplementedException();
        }

    }
}
