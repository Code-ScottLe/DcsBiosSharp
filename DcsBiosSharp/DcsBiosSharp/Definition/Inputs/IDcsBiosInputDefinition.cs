﻿using DcsBiosSharp.Connection;

namespace DcsBiosSharp.Definition.Inputs
{
    public interface IDcsBiosInputDefinition
    {
        IModuleInstrument Instrument
        {
            get; set;
        }

        string Name
        {
            get;
        }

        string Description
        {
            get;
        }

        bool HasArgs
        {
            get;
        }

        IDcsBiosCommand CreateCommand(params object[] args);
    }
}
