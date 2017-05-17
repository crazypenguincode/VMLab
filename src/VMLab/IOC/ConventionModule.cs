using System.Reflection;
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
            builder.RegisterModule<AttributedMetadataModule>();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => !t.Name.EndsWith("Singleton"))
                .AsImplementedInterfaces()
                .WithAttributeFiltering();


            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Singleton"))
                .AsImplementedInterfaces()
                .SingleInstance()
                .WithAttributeFiltering();
        }
    }
}
