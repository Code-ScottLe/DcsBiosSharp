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

        public override string GetValueFromBuffer(IReadOnlyList<byte> buffer)
        {
            return Encoding.ASCII.GetString(buffer.Skip((int)Address).Take(MaxLength).ToArray());
        }
    }
}
