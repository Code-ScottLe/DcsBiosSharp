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
        static List<DcsBiosOutput> OptionsDisplay = new List<DcsBiosOutput>();

        static async Task Main(string[] args)
        {

            Console.WriteLine($"Getting modules...");
            ModuleDefinitionManager manager = new ModuleDefinitionManager();
            await manager.RefreshModulesAsync();

            Console.WriteLine($"Found: {manager.Modules.Count} modules.");

            DcsBiosClient client = new DcsBiosClient(manager);
            client.AircraftChanged += (s, e) =>
            {
                Console.WriteLine($"Aircraft changed to: {client.CurrentAircraft?.Name}");
                IEnumerable<IModuleInstrumentDefinition> hornetUfcOptDisplay = client.CurrentAircraft?.Instruments?.Where(i => i.Identifier.Contains("UFC_OPTION_DISPLAY_"));

                if (hornetUfcOptDisplay != null && hornetUfcOptDisplay.Any())
                {
                    foreach(IModuleInstrumentDefinition ufcDisplayDef  in hornetUfcOptDisplay)
                    {
                        DcsBiosOutput output = client.TrackOutput(ufcDisplayDef.OutputDefinitions.Single());
                        output.PropertyChanged += (s, e) => Console.WriteLine($"{output.Definition.Instrument.Identifier}: {output.Value}");

                        OptionsDisplay.Add(output);
                    }
                }
            };

            await client.ConnectAsync();

            Console.WriteLine($"Press any key to quit");
            Console.ReadKey();
        }
    }
}
