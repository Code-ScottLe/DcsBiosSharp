using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Protocol
{
    public class DcsBiosProtocolParser : IDcsBiosProtocolParser
    {
        private (ushort address, List<byte> buffer, int byteLeftToRead) _stagingData;

        private bool IsStaging
        {
            get => _stagingData.byteLeftToRead > 0 && _stagingData.buffer != null;
        }

        private bool _isWaitingForSync;

        public DcsBiosProtocolParser()
        {
            _isWaitingForSync = true;
            _stagingData = (0, null, 0);
        }

        public IReadOnlyList<IDcsBiosExportData> ParseBuffer(IEnumerable<byte> buffer)
        {
            // Start of export protocol.
            int skipIndex = 0;
            List<IDcsBiosExportData> toBeReturned = new List<IDcsBiosExportData>();

            while (skipIndex < buffer.Count())
            {
                // check for sync.
                if (_isWaitingForSync)
                {
                    // if sync at start of the sequence, then it is a brand new write, possible corruption on staging data. Just drop it (if we have it)
                    if (_stagingData.buffer != null)
                    {
                        // log something?
                        _stagingData = (0, null, 0);
                    }

                    int counter = 4;

                    foreach (byte b in buffer)
                    {
                        skipIndex++;
                        if (b != 0x55)
                        {
                            counter = 4;
                            continue;
                        }
                        else
                        {
                            if (--counter == 0)
                            {
                                _isWaitingForSync = false;
                                break;
                            }
                        }
                    }
                }

                else if (IsStaging)
                {
                    // Staging from last read, continue.
                    IEnumerable<byte> leftOver = buffer.Skip(skipIndex).Take(_stagingData.byteLeftToRead);

                    _stagingData.buffer.AddRange(leftOver);

                    _stagingData.byteLeftToRead -= leftOver.Count();

                    if (_stagingData.byteLeftToRead == 0)
                    {
                        var exportData = new DcsBiosExportData(_stagingData.address, _stagingData.buffer);
                        toBeReturned.Add(exportData);
                        _stagingData = (0x0, null, 0);
                    }

                    skipIndex += leftOver.Count();
                }
                else if (buffer is byte[] arrayBuffer)
                {
                    // Little endian buffer. Flip it and convert to address and size.
                    var addressSpan = new Span<byte>(arrayBuffer, skipIndex, 2);
                    ushort address = (ushort)(addressSpan[1] << 8 | addressSpan[0]);

                    if (address == 0x5555)
                    {
                        _isWaitingForSync = true;
                        continue;
                    }

                    skipIndex += 2;

                    var sizeSpan = new Span<byte>(arrayBuffer, skipIndex, 2);
                    ushort size = (ushort)(sizeSpan[1] << 8 | sizeSpan[0]);

                    skipIndex += 2;

                    if (arrayBuffer.Length < skipIndex + size)
                    {
                        // need staging.
                        int byteLeft = arrayBuffer.Length - skipIndex;
                        byte[] stagingBytes = new Span<byte>(arrayBuffer, skipIndex, byteLeft).ToArray();
                        _stagingData = (address, stagingBytes.ToList(), size - stagingBytes.Length);

                        skipIndex += byteLeft;
                    }
                    else
                    {
                        // complete
                        var exportData = new DcsBiosExportData(address, new Span<byte>(arrayBuffer, skipIndex, size).ToArray());
                        toBeReturned.Add(exportData);

                        skipIndex += size;
                    }
                }
                else
                {
                    // Little endian buffer. Flip it and convert to address and size.
                    IEnumerable<byte> addressBuffer = buffer.Skip(skipIndex).Take(2);
                    ushort address = (ushort)(addressBuffer.Last() << 8 | addressBuffer.First());

                    if (address == 0x5555)
                    {
                        _isWaitingForSync = true;
                        continue;
                    }

                    skipIndex += 2;

                    IEnumerable<byte> sizeBuffer = buffer.Skip(skipIndex).Take(2);
                    ushort size = (ushort)(sizeBuffer.Last() << 8 | sizeBuffer.First());

                    skipIndex += 2;

                    IEnumerable<byte> valueBuffer = buffer.Skip(skipIndex).Take(size);
                    if (valueBuffer.Count() < size)
                    {
                        // need staging.
                        _stagingData = (address, valueBuffer.ToList(), size - valueBuffer.Count());
                    }
                    else
                    {
                        // complete package.
                        var exportData = new DcsBiosExportData(address, valueBuffer.ToArray());
                        toBeReturned.Add(exportData);
                    }

                    skipIndex += valueBuffer.Count();
                }
            }

            return toBeReturned;

        }

        public byte[] GetInputBuffer(IDcsBiosCommand command)
        {
            string payload = $"{command.CommandIdentifier} {command.Arguments}\n";
            byte[] buffer = Encoding.ASCII.GetBytes(payload);

            return buffer;
        }
    }
}
