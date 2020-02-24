using System;
using System.Collections.Generic;
using System.Text;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public interface IDcsBiosDataBuffer
    {
        byte[] Buffer
        {
            get;
        }

        event EventHandler<DcsBiosBufferUpdatedEventArgs> BufferUpdated;

        void HandleExportData(IDcsBiosExportData exportData);
    }
}
