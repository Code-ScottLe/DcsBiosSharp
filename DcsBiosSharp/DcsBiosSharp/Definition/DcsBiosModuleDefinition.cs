using System.Collections.Generic;
using System.Linq;

namespace DcsBiosSharp.Definition
{
    public class DcsBiosModuleDefinition : IModuleDefinition
    {
        public string Name
        {
            get; private set;
        }

        public IReadOnlyList<IModuleInstrumentDefinition> Instruments
        {
            get; private set;
        }

        public DcsBiosModuleDefinition(string name, IEnumerable<IModuleInstrumentDefinition> instrumentsDefinitions)
        {
            Name = name;
            Instruments = instrumentsDefinitions is IReadOnlyList<IModuleInstrumentDefinition> listy ? listy : instrumentsDefinitions.ToList();
        }
    }
}
