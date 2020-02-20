using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Definition.Outputs
{
    public interface IDcsBiosOutputDefinition
    {
        IModuleInstrument Instrument
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

        object GetValueFromBuffer(IReadOnlyList<byte> buffer);
    }

    public interface IDcsBiosOutputDefinition<T> : IDcsBiosOutputDefinition
    {
        new T GetValueFromBuffer(IReadOnlyList<byte> buffer);
    }
}
