# DcsBiosSharp
.NET Standard SDK for DCS-BIOS . Currently under WIP (so does this readme). The intention is to provide a modern SDK to interact with DCS-BIOS plugin for DCS World. This project has neither direct relationship with the original creator of DCS-BIOS nor the DCS-BIOS original repo itself.

# Nuget package
Will come around when I stop introducing breaking changes/have time.

# Pre-Requisite (DO NOT SKIP)
1. Download and Install DCS-BIOS, preferred [the default one](https://github.com/dcs-bios/dcs-bios).
2. Config DCS-BIOS through the built-in webapp (with connecting to DCS and downloading plugins/module definitions). An API is planned to do this within the library later.
3. Make sure that DCS can talk to DCS-BIOS (The "Virtual Cockpit" will greenlighted on the dashboard of DCS-BIOS Dashboard section if DCS can communicate with DCS-BIOS).
4. Download and install .NET Core 3.1 SDK if you plan to build the SimpleDcsBiosClient project.

# Example
This is still WIP so anything can be subjected to change. (Check the Unit Tests or the SimpleDcsBiosClient project for latest example)

```
// Initate client (assuming that DCS-BIOS has been downloaded and modules definition has been setup at the default location)
DcsBiosClient client = new DcsBiosClient();
await client.StartAsync();

// Get the output that you want to keep track. In this case, we get all of the Hornet's UFC Option Displays (5 of them);
IEnumerable<DcsBiosOutput> outputs = client.Outputs.Where(o => o.Definition.Instrument.Identifier.Contains("UFC_OPTION_DISPLAY_"));
foreach(var output in outputs)
{
    output.PropertyChanged += (s, e) =>
    {
        // Print out the updated value to console.
        Console.WriteLine($"{(s as DcsBiosOutput).Definition.Instrument.Identifier} : {(s as DcsBiosOutput).Value}");
    });
}

// Command sending in will be in shortly.
```

# License
DcsBiosSharp is under MIT License.