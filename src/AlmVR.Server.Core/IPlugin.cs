using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlmVR.Server.Core
{
    public interface IPlugin
    {
        void Initialize(ContainerBuilder builder);
    }
}
