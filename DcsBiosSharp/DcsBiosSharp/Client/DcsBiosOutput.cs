using System;
using System.Collections.Generic;
using System.Text;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;

namespace DcsBiosSharp.Client
{
    public class DcsBiosOutput : BindableBase, IDisposable
    {
        protected object _value;

        protected bool disposed = false;

        public IDcsBiosOutputDefinition Definition
        {
            get; private set;
        }

        public IDcsBiosDataBuffer Buffer
        {
            get; private set;
        }

        public object Value
        {
            get => _value;
            protected set => Set(ref _value, value);
        }


        public DcsBiosOutput(IDcsBiosOutputDefinition definition, IDcsBiosDataBuffer buffer)
        {
            Definition = definition;
            Buffer = buffer;

            Buffer.BufferUpdated += OnBufferUpdated;
            
        }

        private void OnBufferUpdated(object sender, DcsBiosBufferUpdatedEventArgs e)
        {
            if (Definition.Address >= e.StartIndex && Definition.Address <= e.EndIndex)
            {
                // Update is for us.
                Span<byte> sliced = Buffer.Buffer.AsSpan((int)Definition.Address, Definition.MaxSize);

                Value = Definition.GetValueFromSpan(sliced);
            }
        }

        ~DcsBiosOutput()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                Buffer.BufferUpdated -= OnBufferUpdated;
            }

            disposed = true;
        }
    }

    public class DcsBiosOutput<T> : DcsBiosOutput
    {
        public new IDcsBiosOutputDefinition<T> Definition
        {
            get => base.Definition as IDcsBiosOutputDefinition<T>;
        }

        public new T Value
        {
            get => (T)base.Value;
            protected set => Set(ref _value, value);
        }

        public DcsBiosOutput(IDcsBiosOutputDefinition<T> definition, IDcsBiosDataBuffer buffer) 
            : base(definition, buffer)
        {
        }
    }
}
