using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DcsBiosSharp.Client;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Protocol;

namespace SimpleDcsBiosClient
{
    class Program
    {
        private static MemoryStream memStream;
        private static BinaryWriter writer;

        static async Task Main(string[] args)
        {
            int tick = 0;
            int counter = 0;

            int sizeCounter = 0;

            bool exportBuffer = false;

            DcsBiosClient client = new DcsBiosClient();

            if (exportBuffer)
            {
                memStream = new MemoryStream();
                writer = new BinaryWriter(memStream);
                client.Connection.RawBufferReceived += (s, e) =>
                {
                    // export raw buffer.
                    writer.Write(e);
                    sizeCounter += e.Length;
                };
            }

            // Just to see if DCS is still exporting.
            client.Connection.ExportDataReceived += (s, e) =>
            {
                if (++tick >= 100) // DCS-BIOS does 30 updates per seconds = 0.033s per update. Time 100 result in about 3s per console print. 
                {
                    Console.WriteLine($"Exported {counter += e.Data.Count} bytes! !");
                    tick = 0;
                    counter = 0;
                }
                else
                {
                    counter += e.Data.Count;
                }
                
            };
            await client.StartAsync();

            IEnumerable<DcsBiosOutput> outputs = client.Outputs.Where(o => o.Definition.Instrument.Identifier.Contains("UFC_OPTION_DISPLAY_"));
            foreach(var output in outputs)
            {
                output.PropertyChanged += OutputChanged;
            }

            Console.WriteLine("Waiting for DCS... (type any key to quit)");
            Console.ReadLine();

            if (exportBuffer)
            {
                // Clean up and flush to file.
                writer.Flush();

                using (FileStream streamed = new FileStream("buffer.buff", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (BinaryWriter fileWriter = new BinaryWriter(streamed))
                {
                    fileWriter.Write(memStream.GetBuffer().Take(sizeCounter).ToArray());
                    fileWriter.Flush();
                }

                using (FileStream streamDecoded = new FileStream("buffer.bufferText", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (StreamWriter fileWriter = new StreamWriter(streamDecoded))
                {
                    StringBuilder builder = new StringBuilder();
                    foreach(byte b in memStream.GetBuffer().Take(sizeCounter).ToArray())
                    {
                        builder.Append("0x");
                        builder.Append(b.ToString("x2"));
                        builder.Append(' ');
                    }

                    fileWriter.Write(builder.ToString());
                }

                writer.Dispose();
                memStream.Dispose();
            }
        }

        private static void OutputChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine($"{(sender as DcsBiosOutput).Definition.Instrument.Identifier} : {(sender as DcsBiosOutput).Value}");
        }
    }
}
