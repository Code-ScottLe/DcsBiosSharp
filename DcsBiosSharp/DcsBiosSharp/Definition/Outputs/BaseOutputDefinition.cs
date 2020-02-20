﻿using System;
using System.Collections.Generic;
using System.Text;

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

        public BaseOutputDefinition(uint address, string description = default(string), string suffix = default(string))
        {
            Address = address;
            Description = description;
            Suffix = suffix;
        }

        public abstract T GetValueFromBuffer(IReadOnlyList<byte> buffer);

        object IDcsBiosOutputDefinition.GetValueFromBuffer(IReadOnlyList<byte> buffer)
        {
            return (this as IDcsBiosOutputDefinition<T>).GetValueFromBuffer(buffer);
        }
    }
}
