using System;
using System.Collections.Generic;

namespace DcsBiosSharp.Definition.Outputs
{
    public abstract class BaseOutputDefinition<T> : IDcsBiosOutputDefinition<T>
    {
        public uint Address
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string Suffix
        {
            get; set;
        }

        public abstract int MaxSize
        {
            get;
        }

        public IModuleInstrument Instrument
        {
            get; set;
        }

        public BaseOutputDefinition(uint address, string description = default(string), string suffix = default(string))
        {
            Address = address;
            Description = description;
            Suffix = suffix;
        }

        public abstract T GetValueFromBuffer(IList<byte> buffer);

        object IDcsBiosOutputDefinition.GetValueFromBuffer(IList<byte> buffer)
        {
            return (this as IDcsBiosOutputDefinition<T>).GetValueFromBuffer(buffer);
        }

        public abstract T GetValueFromMemory(Memory<byte> sliced);

        object IDcsBiosOutputDefinition.GetValueFromMemory(Memory<byte> sliced)
        {
            return (this as IDcsBiosOutputDefinition<T>).GetValueFromMemory(sliced);
        }
    }
}
