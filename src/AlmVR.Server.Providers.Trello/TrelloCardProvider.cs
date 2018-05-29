using AlmVR.Common.Models;
using AlmVR.Server.Core.Providers;
using AlmVR.Server.Providers.Trello.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Server.Providers.Trello
{
    internal class TrelloCardProvider : TrelloProviderBase, ICardProvider
    {
        public TrelloCardProvider(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "cards") { }

        public async Task<CardModel> GetCardAsync(string id)
        {
            await InitializeAsync();

            var url = $"{BaseUrl}/{id}?fields=all&{KeyAndToken}";

            string json = null;
            using (var result = await HttpClient.GetAsync(url))
            {
                result.EnsureSuccessStatusCode();

                json = await result.Content.ReadAsStringAsync();
            }

            var trelloCard = JsonConvert.DeserializeObject<TrelloCardModel>(json);

            return new CardModel
            {
                ID = trelloCard.ID,
                Name = trelloCard.Name
            };
        }
    }
}
