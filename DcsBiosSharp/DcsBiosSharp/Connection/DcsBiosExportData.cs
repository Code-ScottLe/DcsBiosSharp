using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Connection
{
    public class DcsBiosExportData : IDcsBiosExportData
    {
        public ushort Address
        {
            get; private set;
        }

        public IReadOnlyList<byte> Data
        {
            get; private set;
        }


        public DcsBiosExportData(ushort address, IReadOnlyList<byte> data)
        {
            Address = address;
            Data = data;
        }
    }
}
