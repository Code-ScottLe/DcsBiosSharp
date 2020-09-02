using System.Collections.Generic;
using System.Threading.Tasks;

namespace DcsBiosSharp.Definition
{
    public interface IModuleDefinitionManager
    {
        string ModuleDefinitionLocation { get; }

        IDictionary<string, IModuleDefinition> Modules { get; }

        IDcsBiosModuleDefinitionJsonParser Parser { get; }

        Task RefreshModulesAsync(string searchPatternOverride = default);
    }
}
