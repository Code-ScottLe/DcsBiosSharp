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

        public IModuleInstrumentDefinition Instrument
        {
            get; set;
        }

        public BaseOutputDefinition(IModuleInstrumentDefinition moduleInstrument, uint address, string description = default(string), string suffix = default(string))
        {
            Instrument = moduleInstrument;
            Address = address;
            Description = description;
            Suffix = suffix;
        }

        public abstract T GetValueFromBuffer(IList<byte> buffer);

        object IDcsBiosOutputDefinition.GetValueFromBuffer(IList<byte> buffer)
        {
            return (this as IDcsBiosOutputDefinition<T>).GetValueFromBuffer(buffer);
        }

        public T GetValueFromMemory(Memory<byte> sliced)
        {
            return GetValueFromSpan(sliced.Span);
        }

        public abstract T GetValueFromSpan(Span<byte> span);

        object IDcsBiosOutputDefinition.GetValueFromMemory(Memory<byte> sliced)
        {
            return (this as IDcsBiosOutputDefinition<T>).GetValueFromMemory(sliced);
        }

        object IDcsBiosOutputDefinition.GetValueFromSpan(Span<byte> span)
        {
            return (this as IDcsBiosOutputDefinition<T>).GetValueFromSpan(span);
        }
    }
}
