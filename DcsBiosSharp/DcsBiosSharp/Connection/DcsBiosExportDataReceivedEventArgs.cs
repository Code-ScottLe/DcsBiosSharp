using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Connection
{
    public class DcsBiosExportDataReceivedEventArgs
    {
        public IReadOnlyList<IDcsBiosExportData> Data
        {
            get; private set;
        }

        public DateTime? ReceivedTime
        {
            get; private set;
        }

        public DcsBiosExportDataReceivedEventArgs(IReadOnlyList<IDcsBiosExportData> exportData, DateTime? receiveTime = default)
            : base()
        {
            Data = exportData;
            ReceivedTime = receiveTime ?? DateTime.UtcNow;
        }
    }
}
