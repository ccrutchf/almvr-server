using AlmVR.Server.Core;
using AlmVR.Server.Core.Providers;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Server.Provider.FileSystem
{
    public class FileSystemPlugin : IPlugin
    {
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystemConfigurationProvider>().As<IConfigurationProvider>();
        }

        public Task InitializeAsync(IContainer container)
        {
            return Task.FromResult(0);
        }
    }
}
