using System;
using System.Collections.Generic;
using System.Text;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public interface IDcsBiosDataBuffer
    {
        IList<byte> Buffer
        {
            get;
        }

        event EventHandler<DcsBiosBufferUpdatedEventArgs> BufferUpdated;

        void HandleExportData(IDcsBiosExportData exportData);
    }
}
