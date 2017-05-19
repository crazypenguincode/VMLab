using System;
using System.Linq;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;
using Module = Autofac.Module;

namespace VMLab.IOC
{
    public class ConventionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var asm = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("VMLab")).ToArray();

            builder.RegisterModule<AttributedMetadataModule>();

            builder.RegisterAssemblyTypes(asm)
                .Where(t => !t.Name.EndsWith("Singleton"))
                .AsImplementedInterfaces()
                .WithAttributeFiltering();


            builder.RegisterAssemblyTypes(asm)
                .Where(t => t.Name.EndsWith("Singleton"))
                .AsImplementedInterfaces()
                .SingleInstance()
                .WithAttributeFiltering();
        }
    }
}
