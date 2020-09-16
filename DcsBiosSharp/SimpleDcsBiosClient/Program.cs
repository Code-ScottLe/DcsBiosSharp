using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DcsBiosSharp.Client;
using DcsBiosSharp.Connection;
using DcsBiosSharp.Definition;
using DcsBiosSharp.Definition.Outputs;
using DcsBiosSharp.Protocol;

namespace SimpleDcsBiosClient
{
    class Program
    {
        static DcsBiosOutput<string> scratchPadOutput;
        static string scratchPadValue;
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Getting modules...");
            ModuleDefinitionManager manager = new ModuleDefinitionManager();
            await manager.RefreshModulesAsync();

            Console.WriteLine($"Found: {manager.Modules.Count} modules.");

            DcsBiosClient client = new DcsBiosClient(manager);
            CancellationTokenSource tokenSrc = new CancellationTokenSource();
            client.AircraftChanged += async (s, e) =>
            {
                Console.WriteLine($"Aircraft changed to: {client.CurrentAircraft?.Name}");
                var scrathpad = client.CurrentAircraft?.Instruments?.FirstOrDefault(i => i.Identifier == "UFC_SCRATCHPAD_NUMBER_DISPLAY");

                scratchPadOutput = client.TrackOutput(scrathpad.OutputDefinitions.Single() as IDcsBiosOutputDefinition<string>);
                scratchPadOutput.PropertyChanged += (s, e) => Console.WriteLine($"Scratchpad: {scratchPadOutput.Value}");


                var opDisplay = client.CurrentAircraft?.Instruments?.Where(i => i.Identifier.Contains("UFC_OPTION_DISPLAY"));
                var trackedOpDisplay = opDisplay.Select(i => client.TrackOutput(i.OutputDefinitions.Single()));

                foreach(var tracked in trackedOpDisplay)
                {
                    tracked.PropertyChanged += (s, e) => Console.WriteLine($"{tracked.Definition.Instrument.Identifier} : {tracked.Value}");
                }
            };

            //client.DcsConnection.RawBufferReceived += (s, e) => Console.WriteLine($"Buf {(float)e.Length / 1000} kb !");

            await client.ConnectAsync();

            Console.WriteLine($"Press any key to quit");
            Console.ReadKey();
            tokenSrc.Cancel();

            Console.WriteLine($"Quit!");
        }
    }
}
