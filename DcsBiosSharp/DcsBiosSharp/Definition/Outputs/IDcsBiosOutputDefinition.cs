using System;
using System.Collections.Generic;

namespace DcsBiosSharp.Definition.Outputs
{
    public interface IDcsBiosOutputDefinition
    {
        IModuleInstrumentDefinition Instrument
        {
            get; set;
        }

        uint Address
        {
            get; set;
        }

        string Description
        {
            get; set;
        }

        string Suffix
        {
            get;
        }

        int MaxSize
        {
            get;
        }

        object GetValueFromBuffer(IList<byte> buffer);

        object GetValueFromMemory(Memory<byte> sliced);

        object GetValueFromSpan(Span<byte> span);
    }

    public interface IDcsBiosOutputDefinition<T> : IDcsBiosOutputDefinition
    {
        new T GetValueFromBuffer(IList<byte> buffer);

        new T GetValueFromMemory(Memory<byte> sliced);

        new T GetValueFromSpan(Span<byte> span);
    }
}
