using System;
using System.Collections.Generic;
using System.Text;

namespace DcsBiosSharp.Connection
{
    public interface IDcsBiosCommand
    {
        string Name
        { 
            get; 
        }

        string Arguments
        {
            get;
        }
    }
}
