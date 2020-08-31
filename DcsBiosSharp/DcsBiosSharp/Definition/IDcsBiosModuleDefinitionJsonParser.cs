namespace DcsBiosSharp.Definition
{
    public interface IDcsBiosModuleDefinitionJsonParser
    {
        IModuleDefinition ParseModuleFromJson(string moduleId, string json);
    }
}