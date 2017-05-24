using VMLab.GraphModels;

namespace VMLab.Contract
{
    public interface ITemplateManager
    {
        bool CanBuild(Template template);
        void Build(Template template, string templateFolder);
        void BuildVMFromTemplate(VM vm);
        void ImportTemplate(string path);
        void RemoveTemplate(string name);

    }
}
