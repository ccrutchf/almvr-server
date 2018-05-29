using AlmVR.Server.Core.Providers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Server.Providers.Trello
{
    public abstract class TrelloProviderBase
    {
        private const string TRELLO_URL = "https://trello.com/1";

        private bool initialized;

        protected string BaseUrl => $"{TRELLO_URL}/{Path}";
        protected string KeyAndToken => $"key={ApiKey}&token={Token}";

        protected string ApiKey { get; set; }
        protected IConfigurationProvider ConfigurationProvider { get; private set; }
        protected HttpClient HttpClient { get; } = new HttpClient();
        protected string Path { get; set; }
        protected string Token { get; set; }

        public TrelloProviderBase(IConfigurationProvider configurationProvider, string path)
        {
            ConfigurationProvider = configurationProvider;
            Path = path;
        }

        public async Task InitializeAsync()
        {
            if (initialized)
                return;

            var config = await ConfigurationProvider.GetConfigurationAsync<TrelloConfiguration>();
            ApiKey = config.ApiKey;
            Token = config.Token;

            initialized = true;
        }
    }
}
