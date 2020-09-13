using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DcsBiosSharp.Definition.Outputs
{
    public class StringOutputDefinition : BaseOutputDefinition<string>
    {
        public int MaxLength
        {
            get; set;
        }

        public override int MaxSize
        {
            get => MaxLength;
        }

        public StringOutputDefinition(IModuleInstrumentDefinition moduleInstrument, uint address, int maxLength, string description = default, string suffix = default)
            : base(moduleInstrument, address, description, suffix)
        {
            MaxLength = maxLength;
        }

        public override string GetValueFromBuffer(IList<byte> buffer)
        {
            if (buffer is byte[] arraybyte)
            {
                return GetValueFromMemory(new Memory<byte>(arraybyte, (int)Address, MaxSize));
            }
            else
            {
                return Encoding.ASCII.GetString(buffer.Skip((int)Address).Take(MaxLength).TakeWhile(b => b != '\0').ToArray());
            }
        }

        public override string GetValueFromSpan(Span<byte> sliced)
        {
            return Encoding.ASCII.GetString(sliced.ToArray().TakeWhile(b => b != '\0').ToArray());
        }
    }
}
