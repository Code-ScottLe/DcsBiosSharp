using System.Collections.Generic;
using System.Linq;

namespace DcsBiosSharp.Definition
{
    public class DcsBiosModule : IModuleDefinition
    {
        public string Name
        {
            get; private set;
        }

        public IReadOnlyList<IModuleInstrumentDefinition> Instruments
        {
            get; private set;
        }

        public DcsBiosModule(string name, IEnumerable<IModuleInstrumentDefinition> instruments)
        {
            Name = name;
            Instruments = instruments is IReadOnlyList<IModuleInstrumentDefinition> listy ? listy : instruments.ToList();
        }
    }
}
