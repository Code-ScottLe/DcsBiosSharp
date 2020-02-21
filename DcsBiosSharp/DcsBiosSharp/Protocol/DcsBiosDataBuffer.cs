using System;
using System.Collections.Generic;
using System.Linq;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public class DcsBiosDataBuffer : IDcsBiosDataBuffer
    {
        public byte[] _buffer;

        public IList<byte> Buffer => _buffer;

        public event EventHandler<DcsBiosBufferUpdatedEventArgs> BufferUpdated;

        public DcsBiosDataBuffer()
        {
            _buffer = new byte[65536]; // 64k byte.
        }

        public virtual void HandleExportData(IDcsBiosExportData exportData)
        {
            byte[] src = exportData.Data is byte[] array ? array : exportData.Data.ToArray();

            Array.Copy(src, 0, _buffer, exportData.Address, exportData.Data.Count);

            BufferUpdated?.Invoke(this, new DcsBiosBufferUpdatedEventArgs(exportData));
        }

        public void OnExportDataReceived(object sender, DcsBiosExportDataReceivedEventArgs args)
        {
            foreach (IDcsBiosExportData exportData in args.Data)
            {
                HandleExportData(exportData);
            }
        }
    }
}
