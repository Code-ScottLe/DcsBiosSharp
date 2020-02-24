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

        protected virtual void OnBufferUpdated(object sender, DcsBiosBufferUpdatedEventArgs e)
        {
            // check if the update is for us
            if (e.StartIndex <= Definition.Address && Definition.Address <= e.EndIndex)
            {
                OnPropertyChanged(nameof(Value));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(propertyName)));
        }
    }

    public class DcsBiosOutput<T> :DcsBiosOutput where T:IEquatable<T>
    {
        private T _cached;

        public DcsBiosOutput(IDcsBiosOutputDefinition<T> definition, IDcsBiosDataBuffer dataBuffer) 
            : base(definition, dataBuffer)
        {
        }

        public new IDcsBiosOutputDefinition<T> Definition
        {
            get => base.Definition as IDcsBiosOutputDefinition<T>;
        }

        public new T Value
        {
            get => _cached;
            private set
            {
                if (_cached == null || (_cached as IEquatable<T>).Equals(value as IEquatable<T>) == false)
                {
                    _cached = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        protected override void OnBufferUpdated(object sender, DcsBiosBufferUpdatedEventArgs e)
        {
            // check if the update is for us
            if (e.StartIndex <= Definition.Address && Definition.Address <= e.EndIndex)
            {
                Value = Definition.GetValueFromMemory(_mem);
            }
        }
    }
}
