﻿namespace VMLab.Contract
{
    public interface ILabManager
    {
        void ExportLab(string path);
        void ImportLab(string path);
        void Clean();
        void Init(string templateName);
    }
}
