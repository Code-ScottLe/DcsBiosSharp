using System;
using System.Linq;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public class DcsBiosDataBuffer
    {
        public byte[] Buffer
        {
            get; private set;
        }

        public event EventHandler<DcsBiosBufferUpdatedEventArgs> BufferUpdated;

        public DcsBiosDataBuffer()
        {
            Buffer = new byte[65536]; // 64k byte.
        }

        public virtual void HandleExportData(IDcsBiosExportData exportData)
        {
            byte[] src = exportData.Data is byte[] array ? array : exportData.Data.ToArray();

            Array.Copy(src, 0, Buffer, exportData.Address, exportData.Data.Count);

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
