using SystemInterface.IO;
using SystemWrapper.IO;
using Autofac;

namespace VMLab.IOC
{
    public class SystemWrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileWrap>().As<IFile>();
            builder.RegisterType<DirectoryWrap>().As<IDirectory>();
        }
    }
}
