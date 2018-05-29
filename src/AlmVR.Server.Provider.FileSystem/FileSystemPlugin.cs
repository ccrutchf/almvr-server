using AlmVR.Server.Core;
using AlmVR.Server.Core.Providers;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlmVR.Server.Provider.FileSystem
{
    public class FileSystemPlugin : IPlugin
    {
        public void Initialize(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystemConfigurationProvider>().As<IConfigurationProvider>();
        }
    }
}
