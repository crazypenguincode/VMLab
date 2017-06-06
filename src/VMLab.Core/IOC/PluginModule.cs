using System;
using System.IO;
using System.Reflection;
using SystemWrapper;
using SystemWrapper.IO;
using Autofac;
using VMLab.Helper;
using Module = Autofac.Module;

namespace VMLab.IOC
{
    public class PluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var config = new Config(new EnvironmentWrap(), new DirectoryWrap(), new FileWrap(), null);
            var hypervisor = config.GetSetting("Hypervisor");           
            var asmFolder = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            var pluginPath = $"{asmFolder}\\VMLab.Hypervisor.{hypervisor}.dll";

            if (!File.Exists(pluginPath)) return;
            
            Assembly AsmResolve(object sender, ResolveEventArgs args) => 
                File.Exists($"{asmFolder}\\{args.Name}.dll") ? Assembly.Load($"{asmFolder}\\{args.Name}.dll") : null;
                
            AppDomain.CurrentDomain.AssemblyResolve += AsmResolve;
                
            var asm = Assembly.LoadFile(pluginPath);

            AppDomain.CurrentDomain.AssemblyResolve -= AsmResolve;

            builder.RegisterAssemblyModules(asm);
        }
    }
}
