using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;

namespace DcsBiosSharp.Client
{
    public class DcsBiosOutput : INotifyPropertyChanged
    {
        protected Memory<byte> _mem;

        public IDcsBiosOutputDefinition Definition
        {
            get; private set;
        }

        public object Value
        {
            get => Definition.GetValueFromMemory(_mem);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal DcsBiosOutput(IDcsBiosOutputDefinition definition, byte[] buffer)
            : this (definition, new Memory<byte>(buffer, (int)definition.Address, (int)definition.MaxSize))
        {
        }

        internal DcsBiosOutput(IDcsBiosOutputDefinition definition, Memory<byte> section)
        {
            Definition = definition;
            _mem = section;
        }

        internal void OnRawBufferChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    public class DcsBiosOutput<T> :DcsBiosOutput
    {
        public new IDcsBiosOutputDefinition<T> Definition
        {
            get => base.Definition as IDcsBiosOutputDefinition<T>;
        }

        public new T Value
        {
            get => Definition.GetValueFromMemory(_mem);
        }


        internal DcsBiosOutput(IDcsBiosOutputDefinition<T> definition, byte[] buffer)
            : base(definition, buffer)
        {
        }

        internal DcsBiosOutput(IDcsBiosOutputDefinition<T> definition, Memory<byte> section)
            : base (definition, section)
        {
        }
    }
}
