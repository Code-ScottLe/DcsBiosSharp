using System;
using System.Threading.Tasks;

namespace DcsBiosSharp.Connection
{
    public interface IDcsBiosConnection
    {
        event EventHandler<byte[]> RawBufferReceived;

        event EventHandler<DcsBiosExportDataReceivedEventArgs> ExportDataReceived;

        void Start();

        Task SendCommandAsync(IDcsBiosCommand command);
    }
}
