using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DcsBiosSharp.Definition
{
    public class DcsBiosModule : IModule
    {
        public string Name
        {
            get; private set;
        }

        public IReadOnlyList<IModuleInstrument> Instruments
        {
            get; private set;
        }

        public DcsBiosModule(string name, IEnumerable<IModuleInstrument> instruments)
        {
            Name = name;
            Instruments = instruments is IReadOnlyList<IModuleInstrument> listy ? listy : instruments.ToList();
        }
    }
}
