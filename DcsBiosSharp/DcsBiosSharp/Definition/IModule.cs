using System.Collections.Generic;

namespace DcsBiosSharp.Definition
{
    public interface IModule
    {
        string Name
        {
            get;
        }

        IReadOnlyList<IModuleInstrument> Instruments
        {
            get;
        }
    }
}
