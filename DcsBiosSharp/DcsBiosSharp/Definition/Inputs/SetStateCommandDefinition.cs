using DcsBiosSharp.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Definition.Inputs
{
    public abstract class SetStateCommandDefinition : IDcsBiosInputDefinition
    {
        public const string DEFAULT_COMMAND_INTERFACE_NAME = "set_state";

        public string Name
        {
            get => DEFAULT_COMMAND_INTERFACE_NAME;
        }

        public string Description
        {
            get; private set;
        }

        public bool HasArgs => true;

        protected SetStateCommandDefinition(string description)
        {
            Description = description;
        }

        public abstract IDcsBiosCommand CreateCommand(params object[] args);
    }

    public class SetState<T> : SetStateCommandDefinition where T : IComparable
    {
        public T MaxValue
        {
            get; private set;
        }

        public SetState(T maxValue, string description)
            : base(description)
        {
            MaxValue = maxValue;
        }

        public override IDcsBiosCommand CreateCommand(params object[] args)
        {
            //Expect a single input.

            if (args.Length != 1)
            {
                throw new ArgumentOutOfRangeException("SetState expected only one argument");
            }
            else if (!(args[0] is T value))
            {
                throw new ArgumentException($"SetState<{typeof(T).Name}> was given args with incompatible type {args.GetType().Name}");
            }
            else if (value.CompareTo(MaxValue) > 0)
            {
                throw new ArgumentOutOfRangeException($"args is exceed the max value based on definition.");
            }
            else
            {
                var instance = new DcsBiosCommand(DEFAULT_COMMAND_INTERFACE_NAME, value.ToString(), this);
                return instance;
            }
        }
    }
}
