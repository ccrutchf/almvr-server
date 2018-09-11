using AlmVR.Common.Models;
using AlmVR.Server.Core;
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
    public class TrelloCardProvider : TrelloProviderBase, ICardProvider
    {
        private readonly TrelloWebHookProvider webHookProvider;

        public TrelloCardProvider(TrelloWebHookProvider webHookProvider, IConfigurationProvider configurationProvider)
            : base(configurationProvider, "cards")
        {
            this.webHookProvider = webHookProvider;
        }

        public event EventHandler<CardChangedEventArgs> CardChanged;

        public async Task<CardModel> GetCardAsync(string id)
        {
            await InitializeAsync();

            var url = $"{BaseUrl}/{id}?fields=all&{KeyAndToken}";

            string json = null;
            using (var response = await HttpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();

                json = await response.Content.ReadAsStringAsync();
            }

            var trelloCard = JsonConvert.DeserializeObject<TrelloCardModel>(json);

            await webHookProvider.SetAsync(new TrelloWebHook
            {
                IdModel = trelloCard?.ID,
                CallbackURL = $"{HostedUrl}/api/trellocard"
            });

            return trelloCard?.ToCommonCardModel();
        }

        public async Task MoveCardAsync(CardModel card, SwimLaneModel targetSwimLane)
        {
            await InitializeAsync();

            var url = $"{BaseUrl}/{card.ID}/idList?value={targetSwimLane.ID}&{KeyAndToken}";
            
            using (var content = new StringContent(string.Empty))
            using (var response = await HttpClient.PutAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        internal void RaiseCardChanged(TrelloCardModel trelloCardModel)
        {
            CardChanged?.Invoke(this, new CardChangedEventArgs(trelloCardModel.ToCommonCardModel()));
        }
    }
}
