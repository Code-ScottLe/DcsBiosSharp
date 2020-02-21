using System;
using System.Collections.Generic;
using System.Text;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Protocol;

namespace DcsBiosSharp.Client
{
    public class DcsBiosClient
    {
        public IDcsBiosConnection Connection
        {
            get; private set;
        }

        public IDcsBiosDataBuffer DataBuffer
        {
            get; private set;
        }

        public DcsBiosClient(IDcsBiosConnection connection, IDcsBiosDataBuffer dataBuffer)
        {
            Connection = connection;
            Connection.ExportDataReceived += OnConnectionReceivedExportData;

            DataBuffer = dataBuffer;
            DataBuffer.BufferUpdated += OnBufferUpdated;
        }

        private void OnBufferUpdated(object sender, DcsBiosBufferUpdatedEventArgs e)
        {
            
        }

        private void OnConnectionReceivedExportData(object sender, DcsBiosExportDataReceivedEventArgs e)
        {
            foreach (IDcsBiosExportData exportData in e.Data)
            {
                DataBuffer.HandleExportData(exportData);
            }
        }
    }
}
