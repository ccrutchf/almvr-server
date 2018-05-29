using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace AlmVR.Server.Core
{
    public class ServiceProviderFactory
    {
        private static bool containerCreated;

        public static IServiceProvider GetServiceProvider(IServiceCollection services)
        {
            if (containerCreated)
                throw new NotSupportedException("GetServiceProvider can only be called once.");

            containerCreated = true;

            var builder = new ContainerBuilder();
            builder.Populate(services);
            LoadPlugins(builder);

            var container = builder.Build();
            //Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
        }

        private static void LoadPlugins(ContainerBuilder builder)
        {
            var exeLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pluginLocation = Path.Combine(exeLocation, "Plugins");

            var plugins = Directory.GetFiles(pluginLocation, "*.dll", SearchOption.AllDirectories)
              .Select(path => AssemblyLoadContext.Default.LoadFromAssemblyPath(path))
              .Where(x => x != null)
              .Select(x => x.GetTypes())
              .SelectMany(x => x)
              .Where(x => x.GetInterfaces().Contains(typeof(IPlugin)))
              .Select(x => Activator.CreateInstance(x) as IPlugin);

            foreach (var plugin in plugins)
            {
                plugin.Initialize(builder);
            }
        }
    }
}
