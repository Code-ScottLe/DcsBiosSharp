using System;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public class DcsBiosBufferUpdatedEventArgs : EventArgs
    {
        public int StartIndex
        {
            get; private set;
        }

        public int EndIndex
        {
            get; private set;
        }

        public DcsBiosBufferUpdatedEventArgs(IDcsBiosExportData exportData)
            : this(exportData.Address, exportData.Address + exportData.Data.Count - 1)
        {
        }

        public DcsBiosBufferUpdatedEventArgs(int startIndex, int endIndex)
            : base()
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }
}
