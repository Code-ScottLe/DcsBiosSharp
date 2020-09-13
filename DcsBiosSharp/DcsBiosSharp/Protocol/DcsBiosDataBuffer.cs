using System;
using System.Collections.Generic;
using System.Linq;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public class DcsBiosDataBuffer : IDcsBiosDataBuffer
    {
        public byte[] _buffer;

        public byte[] Buffer => _buffer;

        public event EventHandler<DcsBiosBufferUpdatedEventArgs> BufferUpdated;

        private Dictionary<int, List<Action>> _tracker;

        public DcsBiosDataBuffer()
        {
            _buffer = new byte[65536]; // 64k byte.
            _tracker = new Dictionary<int, List<Action>>();
        }

        public DcsBiosDataBuffer(IDcsBiosConnection connection)
            : this ()
        {
            connection.ExportDataReceived += OnExportDataReceived;
        }

        public virtual void HandleExportData(IDcsBiosExportData exportData)
        {
            byte[] src = exportData.Data is byte[] array ? array : exportData.Data.ToArray();

            Array.Copy(src, 0, _buffer, exportData.Address, exportData.Data.Count);

            BufferUpdated?.Invoke(this, new DcsBiosBufferUpdatedEventArgs
                (exportData));

            // check for registered to see which one to notify.
        }

        public Memory<byte> GetTrackingSlicedMemory(int startIndex, int length, Action callBack)
        {
            Memory<byte> sliced = new Memory<byte>(_buffer, startIndex, length);

            if (!_tracker.ContainsKey(startIndex))
            {
                _tracker[startIndex] = new List<Action>();
            }

            _tracker[startIndex].Add(callBack);

            return sliced;
        }

        private void OnExportDataReceived(object sender, DcsBiosExportDataReceivedEventArgs args)
        {
            foreach (IDcsBiosExportData exportData in args.Data)
            {
                HandleExportData(exportData);
            }

        }
    }
}
