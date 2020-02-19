using DcsBiosSharp.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DcsBiosSharp.Connection
{
    public sealed class DcsBiosUdpConnection : IDcsBiosConnection, IDisposable
    {
        private static readonly int DEFAULT_DCS_BIOS_LISTENER_PORT = 7778;
        private static readonly string DEFAULT_DCS_BIOS_MULTICAST_IP = "239.255.50.10";
        private static readonly int DEFAULT_DCS_BIOS_MULTICAST_PORT = 5010;

        private UdpClient _client;
        private UdpClient _exportListener;
        private IPEndPoint _dcsBiosReceivingEndpoint;
        private IPEndPoint _dcsBiosExportingEndpoint;

        private Task _exportingListenerTask;
        private Task _exportParserTask;

        private TaskCompletionSource<object> _signalToken;
        private CancellationTokenSource _tokenSource;

        private ConcurrentQueue<byte[]> _internalBuffer;

        private IDcsBiosProtocolParser Protocol
        {
            get; set;
        }

        public event EventHandler<DcsBiosExportDataReceivedEventArgs> ExportDataReceived;

        public DcsBiosUdpConnection()
            : this(new DcsBiosProtocolParser())
        {
        }

        public DcsBiosUdpConnection(IDcsBiosProtocolParser parser)
        {
            // To do : re-enable user-configurable later.
            _internalBuffer = new ConcurrentQueue<byte[]>();

            // Setup sender.
            _client = new UdpClient();
            _client.ExclusiveAddressUse = false;
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _client.EnableBroadcast = true;
            _dcsBiosReceivingEndpoint = new IPEndPoint(IPAddress.Loopback, DEFAULT_DCS_BIOS_LISTENER_PORT);

            // Setup listening
            _dcsBiosExportingEndpoint = new IPEndPoint(IPAddress.Any, DEFAULT_DCS_BIOS_MULTICAST_PORT);
            _exportListener = new UdpClient();
            _exportListener.ExclusiveAddressUse = false;
            _exportListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _exportListener.Client.Bind(_dcsBiosExportingEndpoint);
            _exportListener.JoinMulticastGroup(IPAddress.Parse(DEFAULT_DCS_BIOS_MULTICAST_IP));

            //Setup polling thread.
            _tokenSource = new CancellationTokenSource();

            Protocol = parser;
        }

        ~DcsBiosUdpConnection()
        {
            this.Dispose();
        }

        public async Task SendCommandAsync(IDcsBiosCommand command)
        {
            byte[] buffer = Protocol.GetInputBuffer(command);

            await _client.SendAsync(buffer, buffer.Length, _dcsBiosReceivingEndpoint);
        }

        public void Start()
        {
            if (_exportingListenerTask == null)
            {
                _exportingListenerTask = Task.Factory.StartNew(PollingForDataAsync, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                _exportParserTask = Task.Factory.StartNew(ParseAndNotifyAsync, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }

        }

        private async Task PollingForDataAsync()
        {
            try
            {
                while (!_tokenSource.IsCancellationRequested)
                {
                    // The lua will send an update roughly 30 times per second
                    UdpReceiveResult result = await _exportListener.ReceiveAsync().ConfigureAwait(false);
                    _internalBuffer.Enqueue(result.Buffer);
                    if (!_signalToken.Task.IsCompleted)
                    {
                        _signalToken.SetResult(true);
                    }
                }

                //exited. peaceful release?
            }
            finally
            {
            }
        }

        private async Task ParseAndNotifyAsync()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                // Has something in the buffer. 
                while (_internalBuffer.TryDequeue(out byte[] buffer))
                {
                    IReadOnlyList<IDcsBiosExportData> data = Protocol.ParseBuffer(buffer);
                    if (ExportDataReceived != null)
                    {
                        var ignored = Task.Run(() => ExportDataReceived.Invoke(this, new DcsBiosExportDataReceivedEventArgs(data)));
                    }
                }

                _signalToken = new TaskCompletionSource<object>();
                await _signalToken.Task;
            }
        }

        public void Dispose()
        {
            _tokenSource.Cancel();

            _client.Close();
            _client.Dispose();

            _exportListener.Close();
            _exportListener.Dispose();
        }
    }
}
