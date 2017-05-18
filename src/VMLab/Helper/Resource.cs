using System;
using System.IO;
using System.Reflection;

namespace VMLab.Helper
{
    public class Resource : IResource
    {
        public string GetText(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            var asm = Assembly.GetExecutingAssembly();

            var reader = new StreamReader(asm.GetManifestResourceStream(name));
            return reader.ReadToEnd();

        }
    }
}
