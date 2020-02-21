namespace DcsBiosSharp.Definition
{
    public interface IDcsBiosModuleDefinitionJsonParser
    {
        IModule ParseModuleFromJson(string moduleId, string json);
    }
}