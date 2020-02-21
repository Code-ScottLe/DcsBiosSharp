using System.Collections.Generic;

namespace DcsBiosSharp.Connection
{
    public interface IDcsBiosExportData
    {
        ushort Address
        {
            get;
        }

        IReadOnlyList<byte> Data
        {
            get;
        }
    }
}
