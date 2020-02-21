using System;
using System.Collections.Generic;
using System.Linq;

namespace DcsBiosSharp.Definition.Outputs
{
    public class IntegerOutputDefinition : BaseOutputDefinition<int>
    {
        public int Mask
        {
            get; set;
        }

        public int MaxValue
        {
            get; set;
        }

        public int ShiftBy
        {
            get; set;
        }

        public override int MaxSize
        {
            get => sizeof(ushort); //2 bytes
        }

        public IntegerOutputDefinition(uint address, int shiftBy, int mask, int maxValue = int.MaxValue, string description = default, string suffix = default)
            : base(address, description, suffix)
        {
            ShiftBy = shiftBy;
            MaxValue = maxValue;
        }

        public override int GetValueFromBuffer(IReadOnlyList<byte> buffer)
        {
            // Flip the byte around as it is litte endian
            ushort raw = BitConverter.ToUInt16(buffer.Skip((int)Address).Take(2).Reverse().ToArray(), 0);

            // masking.
            int masked = (raw & Mask) >> ShiftBy;

            if (masked > MaxValue)
            {
                throw new ArgumentOutOfRangeException($"Supplied value goes out of range compre to max value! MaxValue: {MaxValue} , parsed: {masked}");
            }

            return masked;
        }
    }
}
