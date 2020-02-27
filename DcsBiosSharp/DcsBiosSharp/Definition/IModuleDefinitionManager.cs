using System.Collections.Generic;
using System.Threading.Tasks;

namespace DcsBiosSharp.Definition
{
    public interface IModuleDefinitionManager
    {
        string ModuleDefinitionLocation { get; }

        IList<IModule> Modules { get; }

        IDcsBiosModuleDefinitionJsonParser Parser { get; }

        Task RefreshModuleAsync(string searchPatternOverride = default);
    }
}
