﻿using VMLab.GraphModels;
using VMLab.Script.FluentInterface;

namespace VMLab.Contract
{
    public interface IVMBuilder
    {
        bool CanBuild(Template template);
        void Build(Template template, string templateFolder);
        bool TemplateExist(string name);
        void BuildVMFromTemplate(VM vm);
        IVMControl GetVM(VM vm);

        void ImportTemplate(string path);
        void RemoveTemplate(string name);
        void DestroyVM(VM vm, IVMControl control);
    }
}
