using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DcsBiosSharp.Definition
{
    public class ModuleDefinitionManager : IModuleDefinitionManager
    {
        public static readonly string DEFAULT_DCS_BIOS_MODULE_DEFINITION_LOCATION = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + @"/AppData/Roaming/DCS-BIOS/control-reference-json/";
        public const string DEFAULT_MODULE_FOLDER_SEARCH_PATTERN = @"*.json";

        public string ModuleDefinitionLocation
        {
            get; private set;
        }

        public IList<IModule> Modules
        {
            get; private set;
        }

        public IDcsBiosModuleDefinitionJsonParser Parser
        {
            get; private set;
        }

        public string SearchPattern
        {
            get; set;
        }

        public ModuleDefinitionManager()
            : this(DEFAULT_DCS_BIOS_MODULE_DEFINITION_LOCATION, DEFAULT_MODULE_FOLDER_SEARCH_PATTERN)
        {
        }

        public ModuleDefinitionManager(string moduleDefinitionLocation)
            : this(moduleDefinitionLocation, DEFAULT_MODULE_FOLDER_SEARCH_PATTERN)
        {
        }

        public ModuleDefinitionManager(string moduleDefinitionLocation, string searchPattern)
            : this(moduleDefinitionLocation, searchPattern, new DcsBiosModuleDefinitionJsonParser())
        {
        }

        public ModuleDefinitionManager(string moduleDefinitionsLocation, string searchPattern, IDcsBiosModuleDefinitionJsonParser parser)
        {
            ModuleDefinitionLocation = moduleDefinitionsLocation;
            Parser = parser;
            SearchPattern = searchPattern;
            Modules = new List<IModule>();
        }

        public async Task RefreshModuleAsync(string searchPatternOverride = default)
        {
            DirectoryInfo info = new DirectoryInfo(ModuleDefinitionLocation);
            if (!info.Exists)
            {
                // should we throw?
                return;
            }

            // look for all jsons
            FileInfo[] files = info.GetFiles(searchPatternOverride ?? SearchPattern);

            foreach (var file in files.Where(i => !Modules.Any(m => m.Name == Path.GetFileNameWithoutExtension(i.FullName))))
            {
                string moduleId = Path.GetFileNameWithoutExtension(file.FullName);

                using (StreamReader streamReader = file.OpenText())
                {
                    string json = await streamReader.ReadToEndAsync();
                    IModule module = await Task.Run(() => Parser.ParseModuleFromJson(moduleId, json));
                    Modules.Add(module);
                }
            }


        }

        public IModule GetModule(string moduleIdentifier)
        {
            return Modules.FirstOrDefault(m => m.Name == moduleIdentifier);
        }
    }
}
