using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Server.Core
{
    public interface IPlugin
    {
        void ConfigureContainer(ContainerBuilder builder);
        Task InitializeAsync(IContainer container);
    }
}
