using AlmVR.Server.Core;
using AlmVR.Server.Core.Providers;
using AlmVR.Server.Providers.Trello.Models;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

[assembly: ControllerAssembly]

namespace AlmVR.Server.Providers.Trello
{
    public class TrelloPlugin : IPlugin
    {
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<TrelloBoardProvider>().As<IBoardProvider>().SingleInstance();
            builder.RegisterType<TrelloCardProvider>().As<ICardProvider>().SingleInstance();

            builder.RegisterType<TrelloCardProvider>().As<TrelloCardProvider>().SingleInstance();
            builder.RegisterType<TrelloWebHookProvider>().As<TrelloWebHookProvider>().SingleInstance();
        }

        public Task InitializeAsync(IContainer container)
        {
            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //    TypeNameHandling = TypeNameHandling.Objects,
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};

            return container.Resolve<TrelloWebHookProvider>().PurgeWebHooksAsync();
        }
    }
}
