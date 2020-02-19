using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DcsBiosSharp.Connection
{
    public interface IDcsBiosConnection
    {
        event EventHandler<DcsBiosExportDataReceivedEventArgs> ExportDataReceived;

        void Start();

        Task SendCommandAsync(IDcsBiosCommand command);
    }
}
