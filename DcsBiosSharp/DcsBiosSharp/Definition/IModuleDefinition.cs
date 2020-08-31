using System.Collections.Generic;

namespace DcsBiosSharp.Definition
{
    public interface IModuleDefinition
    {
        string Name
        {
            get;
        }

        IReadOnlyList<IModuleInstrumentDefinition> Instruments
        {
            get;
        }
    }
}
