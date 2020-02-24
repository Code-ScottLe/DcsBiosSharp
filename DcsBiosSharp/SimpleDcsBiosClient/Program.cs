using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static async Task Main(string[] args)
        {
            int tick = 0;
            int counter = 0;

            DcsBiosClient client = new DcsBiosClient();
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
        }

        private static void OutputChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine($"{(sender as DcsBiosOutput).Definition.Instrument.Identifier} : {(sender as DcsBiosOutput).Value}");
        }
    }
}
