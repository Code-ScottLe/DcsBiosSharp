using DcsBiosSharp.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

                    IEnumerable<byte> sizeBuffer = buffer.Skip(skipIndex + 2).Take(2);
                    ushort size = (ushort)(sizeBuffer.Last() << 8 | sizeBuffer.First());

                    IEnumerable<byte> valueBuffer = buffer.Skip(skipIndex + 4).Take(size);
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

                    // 4 bytes for 2 ushort (address and size) + size of the data to skip in the buffer to get to the next point.
                    skipIndex += (4 + valueBuffer.Count());
                }
            }

            return toBeReturned;

        }
    }
}
