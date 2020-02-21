using System.Collections.Generic;

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
