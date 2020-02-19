using System;
using System.Collections.Generic;
using System.Text;

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
