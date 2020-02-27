using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;

namespace DcsBiosSharp.Client
{
    public class DcsBioscClientOutputsChangedEventArgs : EventArgs
    {
        public IReadOnlyList<IDcsBiosOutputDefinition> ChangedOutputs
        {
            get; private set;
        }

        public IDcsBiosDataBuffer Buffer
        {
            get; private set;
        }

        public DcsBioscClientOutputsChangedEventArgs(IDcsBiosDataBuffer buffer, IEnumerable<IDcsBiosOutputDefinition> outputs)
        {
            ChangedOutputs = outputs is IReadOnlyList<IDcsBiosOutputDefinition> def ? def : outputs.ToList();
            Buffer = buffer;
        }
    }
}
