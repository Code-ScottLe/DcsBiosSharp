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

        public IntegerOutputDefinition(IModuleInstrumentDefinition moduleInstrument, uint address, int shiftBy, int mask, int maxValue = int.MaxValue, string description = default, string suffix = default)
            : base(moduleInstrument, address, description, suffix)
        {
            ShiftBy = shiftBy;
            MaxValue = maxValue;
        }

        public override int GetValueFromBuffer(IList<byte> buffer)
        {
            if (buffer is byte[] arraybyte)
            {
                return GetValueFromMemory(new Memory<byte>(arraybyte, (int)Address, MaxSize));
            }
            else
            {
                // Flip the byte around as it is litte endian
                ushort raw = BitConverter.ToUInt16(buffer.Skip((int)Address).Take(2).Reverse().ToArray(), 0);

                return ProcessRawNumber(raw);
            }
            
        }

        public override int GetValueFromMemory(Memory<byte> sliced)
        {
            ushort raw = BitConverter.ToUInt16(sliced.ToArray().Reverse().ToArray(), 0);

            return ProcessRawNumber(raw);
        }

        private int ProcessRawNumber(ushort raw)
        {
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
