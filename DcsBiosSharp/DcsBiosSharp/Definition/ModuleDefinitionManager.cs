using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DcsBiosSharp.Definition
{
    public class ModuleDefinitionManager
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

        public ModuleDefinitionManager()
            : this (DEFAULT_DCS_BIOS_MODULE_DEFINITION_LOCATION, new DcsBiosModuleDefinitionJsonParser())
        {
        }

        public ModuleDefinitionManager(string moduleDefinitionsLocation , IDcsBiosModuleDefinitionJsonParser parser)
        {
            ModuleDefinitionLocation = moduleDefinitionsLocation;
            Parser = parser;
            Modules = new List<IModule>();
        }

        public async Task RefreshModuleAsync(string searchPattern = DEFAULT_MODULE_FOLDER_SEARCH_PATTERN)
        {
            DirectoryInfo info = new DirectoryInfo(ModuleDefinitionLocation);
            if(!info.Exists)
            {
                // should we throw?
                return;
            }

            // look for all jsons
            FileInfo[] files = info.GetFiles(searchPattern);

            foreach(var file in files)
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
    }
}
