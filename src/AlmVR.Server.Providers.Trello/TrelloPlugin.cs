using AlmVR.Server.Core;
using AlmVR.Server.Core.Providers;
using Autofac;
using System;

namespace AlmVR.Server.Providers.Trello
{
    public class TrelloPlugin : IPlugin
    {
        public void Initialize(ContainerBuilder builder)
        {
            builder.RegisterType<TrelloBoardProvider>().As<IBoardProvider>();
            builder.RegisterType<TrelloCardProvider>().As<ICardProvider>();
        }
    }
}
