using System;
using System.IO;
using System.Text;
using Autofac;

namespace VMLab.IOC
{
    public class Bootstrap
    {
        private readonly IContainer _container;

        public Bootstrap()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<PluginModule>();
            builder.RegisterModule<ConventionModule>();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<SystemWrapperModule>();
            _container = builder.Build();

            var sb = new StringBuilder();

            sb.AppendLine("Loaded Assemblies:");
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                sb.AppendLine(a.FullName);

            sb.AppendLine("DI Registrations:");
            foreach (var r in _container.ComponentRegistry.Registrations)
            {
                sb.AppendLine("====================================================================================");
                sb.AppendLine(r.ToString());
            }

            File.WriteAllText(Environment.ExpandEnvironmentVariables("%temp%\\autofactdump.log"), sb.ToString());

        }

        public T Start<T>() => _container.Resolve<T>();
    }
}
