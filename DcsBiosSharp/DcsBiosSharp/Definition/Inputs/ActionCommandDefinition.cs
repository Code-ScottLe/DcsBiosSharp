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

        public virtual bool HasArgs => true;
        
        public ActionCommandDefinition(string description = default(string))
        {
            Description = description;
        }

        public IDcsBiosCommand CreateCommand(params object[] args)
        {
            string commandArgs = string.Join(" ", args).Trim();

            var command = new DcsBiosCommand(DEFAULT_COMMAND_INTERFACE_NAME, commandArgs, this);

            return command;
        }
    }
}
