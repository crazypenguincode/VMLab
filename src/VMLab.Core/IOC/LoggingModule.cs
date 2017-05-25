using System;
using System.IO;
using Autofac;
using Serilog;
using Serilog.Filters;

namespace VMLab.IOC
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var logfolder = $"{Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Process)}\\vmlab";

            if (!Directory.Exists(logfolder))
                Directory.CreateDirectory(logfolder);

            builder.RegisterInstance<ILogger>(new LoggerConfiguration()
                .WriteTo.RollingFile(logfolder + "\\VMLab-{Date}.log")
                .WriteTo.Logger(conlog => conlog
                    .Filter.ByIncludingOnly(Matching.WithProperty<bool>("Console", p => p))
                    .WriteTo.LiterateConsole(outputTemplate: "{Message}\n")
                ).CreateLogger())
                .SingleInstance();
        }
    }
}
