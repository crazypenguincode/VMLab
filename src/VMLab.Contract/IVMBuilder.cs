using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract
{
    public interface IVMBuilder
    {
        bool CanBuild(Template template);
        void Build(Template template, string templateFolder);
    }
}
