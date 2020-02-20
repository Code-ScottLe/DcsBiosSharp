using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            IDcsBiosConnection connection = new DcsBiosUdpConnection(); // default everything.

            DcsBiosDataBuffer buffer = new DcsBiosDataBuffer();
            connection.ExportDataReceived += buffer.OnExportDataReceived;
            connection.ExportDataReceived += (s, e) =>
            {
                if (++tick >= 100) // DCS-BIOS does 30 updates per seconds = 0.033s per update. Time 100 result in about 3s per console print. Just to see if DCS is still exporting.
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

            IModule module = GetModule();
            var scratchPads = module.Instruments.Where(i => i.Identifier.Contains("UFC_OPTION_DISPLAY_"));

            buffer.BufferUpdated += (s, e) =>
            {
                var refresh = scratchPads.SelectMany(s => s.OutputDefinitions).Where(o => e.StartIndex <= o.Address && o.Address + o.MaxSize <= e.EndIndex);
                foreach (var refreshOutput in refresh)
                {
                    string value = refreshOutput.GetValueFromBuffer(buffer.Buffer) as string;
                    Console.WriteLine($"{refreshOutput.Instrument.Identifier} : {value}");
                }
                if(refresh.Any())
                {
                    Console.WriteLine();
                }
            };

            connection.Start();

            Console.WriteLine("Waiting for DCS... (type any key to quit)");
            Console.ReadLine();
        }

        private static IModule GetModule()
        {
            // Hornet for now.
            string json = File.ReadAllText("./FA-18C_hornet.json");

            var parser = new DcsBiosModuleDefinitionJsonParser();

            return parser.ParseModuleFromJson("FA-18C_hornet", json);
        }
    }
}
