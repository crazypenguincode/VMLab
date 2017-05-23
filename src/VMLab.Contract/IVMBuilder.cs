using System.Collections.Generic;
using VMLab.Contract.GraphModels;
using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract
{
    public interface IVMBuilder
    {
        bool CanBuild(Template template);
        void Build(Template template, string templateFolder);
        void BuildVMFromTemplate(VM vm);
        IVMControl GetVM(VM vm);
        TemplateManifest GetTemplateManifestFromArchive(string path);
        IEnumerable<TemplateManifest> GetInstalledTemplateManifests();
        void ImportTemplate(string path);
        void RemoveTemplate(string name);
        void DestroyVM(VM vm, IVMControl control);
        void ExportLab(string path);
        void ImportLab(string path);
    }
}
