using System.Collections.Generic;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public interface IDcsBiosProtocolParser
    {
        IReadOnlyList<IDcsBiosExportData> ParseBuffer(IEnumerable<byte> buffer);

        byte[] GetInputBuffer(IDcsBiosCommand command);
    }
}
