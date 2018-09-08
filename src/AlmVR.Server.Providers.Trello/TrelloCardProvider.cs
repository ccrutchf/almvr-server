using AlmVR.Common.Models;
using AlmVR.Server.Core.Providers;
using AlmVR.Server.Providers.Trello.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public async Task MoveCardAsync(CardModel card, SwimLaneModel targetSwimLane)
        {
            await InitializeAsync();

            var url = $"{BaseUrl}/{card.ID}/idList?value={targetSwimLane.ID}&{KeyAndToken}";
            
            using (var content = new StringContent(string.Empty))
            using (var result = await HttpClient.PutAsync(url, content))
            {
                var response = await result.Content.ReadAsStringAsync();

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
