using DcsBiosSharp.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Protocol
{
    public interface IDcsBiosProtocolParser
    {
        IReadOnlyList<IDcsBiosExportData> ParseBuffer(IEnumerable<byte> buffer);
    }
}
