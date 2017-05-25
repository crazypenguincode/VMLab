using System;
using System.Linq;
using System.Reflection;
using SystemInterface.IO;
using SystemWrapper.IO;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;
using Module = Autofac.Module;

namespace VMLab.IOC
{
    public class SystemWrapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Hack: SystemWrapper and SystemInterface assemblies are not loaded in at this point. This forces them to be.
            IFile type = new FileWrap();

            var asms = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.Contains("SystemInterface") || a.FullName.Contains("SystemWrapper"))
                .ToArray();

            builder.RegisterModule<AttributedMetadataModule>();

            builder.RegisterAssemblyTypes(asms)
                .AsImplementedInterfaces()
                .WithAttributeFiltering();
        }
    }
}
