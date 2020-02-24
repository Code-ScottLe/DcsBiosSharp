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

        internal DcsBiosOutput(IDcsBiosOutputDefinition definition, IDcsBiosDataBuffer dataBuffer)
        {
            Definition = definition;
            _mem = new Memory<byte>(dataBuffer.Buffer, (int)Definition.Address, Definition.MaxSize);
            dataBuffer.BufferUpdated += OnBufferUpdated;
        }

        private void OnBufferUpdated(object sender, DcsBiosBufferUpdatedEventArgs e)
        {
            // check if the update is for us
            if (e.StartIndex <= Definition.Address && Definition.Address <= e.EndIndex)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }

    public class DcsBiosOutput<T> :DcsBiosOutput
    {
        internal DcsBiosOutput(IDcsBiosOutputDefinition<T> definition, IDcsBiosDataBuffer dataBuffer) 
            : base(definition, dataBuffer)
        {
        }

        public new IDcsBiosOutputDefinition<T> Definition
        {
            get => base.Definition as IDcsBiosOutputDefinition<T>;
        }

        public new T Value
        {
            get => Definition.GetValueFromMemory(_mem);
        }

    }
}
