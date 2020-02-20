﻿using DcsBiosSharp.Connection;
using System;
using System.Collections.Generic;
using System.Text;

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

        public VariableStepCommandDefinition(double maxValue, double suggestedSteps, string description = default(string))
            : this(description)
        {
            MaxValue = maxValue;
            SuggestedSteps = suggestedSteps;
        }

        protected VariableStepCommandDefinition(string description = default(string))
        {
            Description = description;
        }

        public IDcsBiosCommand CreateCommand(params object[] args)
        {
            throw new NotImplementedException();
        }

    }
}
