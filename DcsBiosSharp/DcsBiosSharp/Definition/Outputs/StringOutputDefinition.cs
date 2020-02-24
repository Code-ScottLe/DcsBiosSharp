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

        public StringOutputDefinition(uint address, int maxLength, string description = default, string suffix = default)
            : base(address, description, suffix)
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
                return Encoding.ASCII.GetString(buffer.Skip((int)Address).Take(MaxLength).ToArray());
            }
        }

        public override string GetValueFromMemory(Memory<byte> sliced)
        {
            return Encoding.ASCII.GetString(sliced.ToArray());
        }
    }
}
