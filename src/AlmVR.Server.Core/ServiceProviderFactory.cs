using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Server.Core
{
    public class ServiceProviderFactory
    {
        private static bool containerCreated;

        public static IServiceProvider GetServiceProvider(IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            if (containerCreated)
                throw new NotSupportedException("GetServiceProvider can only be called once.");

            containerCreated = true;

            var builder = new ContainerBuilder();
            builder.Populate(services);

            var pluginAssemblies = GetAssemblies();

            var controllerAssemblies = GetControllerAssemblies(pluginAssemblies);
            foreach (var controllerAssembly in controllerAssemblies)
            {
                mvcBuilder.AddApplicationPart(controllerAssembly);
            }

            var plugins = GetPlugins(pluginAssemblies);
            AddPluginsToContainer(plugins, builder);

            var container = builder.Build();
            InitializePlugins(plugins, container);
            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var exeLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pluginLocation = Path.Combine(exeLocation, "Plugins");

            return Directory.GetFiles(pluginLocation, "*.dll", SearchOption.AllDirectories)
              .Select(path => AssemblyLoadContext.Default.LoadFromAssemblyPath(path))
              .Where(x => x != null)
              .Select(x => x.GetTypes())
              .SelectMany(x => x)
              .Where(x => x.GetInterfaces().Contains(typeof(IPlugin)))
              .Select(x => x.Assembly)
              .Distinct()
              .ToArray();
        }

        private static IEnumerable<Assembly> GetControllerAssemblies(IEnumerable<Assembly> assemblies) =>
            (from a in assemblies
             where a.GetCustomAttribute<ControllerAssemblyAttribute>() != null
             select a).ToArray();

        private static IEnumerable<IPlugin> GetPlugins(IEnumerable<Assembly> assemblies)
        {
            var exeLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pluginLocation = Path.Combine(exeLocation, "Plugins");

            return assemblies
              .Select(x => x.GetTypes())
              .SelectMany(x => x)
              .Where(x => x.GetInterfaces().Contains(typeof(IPlugin)))
              .Select(x => Activator.CreateInstance(x) as IPlugin)
              .ToArray();
        }

        private static void AddPluginsToContainer(IEnumerable<IPlugin> plugins, ContainerBuilder builder)
        {
            foreach (var plugin in plugins)
            {
                plugin.ConfigureContainer(builder);
            }
        }

        private static void InitializePlugins(IEnumerable<IPlugin> plugins, IContainer container)
        {
            Task.WaitAll((from p in plugins
                          select p.InitializeAsync(container)).ToArray());
        }
    }
}
