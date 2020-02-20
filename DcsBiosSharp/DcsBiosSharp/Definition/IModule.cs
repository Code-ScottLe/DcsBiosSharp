using System;
using System.Collections.Generic;
using System.Text;

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
