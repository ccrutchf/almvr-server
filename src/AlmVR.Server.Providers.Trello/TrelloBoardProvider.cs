using AlmVR.Common.Models;
using AlmVR.Server.Core.Providers;
using AlmVR.Server.Providers.Trello.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AlmVR.Server.Providers.Trello
{
    internal class TrelloBoardProvider : TrelloProviderBase, IBoardProvider
    {
        public TrelloBoardProvider(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "boards") { }

        public async Task<BoardModel> GetBoardAsync()
        {
            await InitializeAsync();

            var config = await ConfigurationProvider.GetConfigurationAsync<TrelloConfiguration>();
            var boardID = config.BoardID;

            var url = $"{BaseUrl}/{boardID}/lists?cards=open&filter=open&fields=name&{KeyAndToken}";

            string json = null;
            using (var result = await HttpClient.GetAsync(url))
            {
                result.EnsureSuccessStatusCode();

                json = await result.Content.ReadAsStringAsync();
            }

            var trelloSwimLanes = JsonConvert.DeserializeObject<IEnumerable<TrelloSwimLaneModel>>(json);

            return new BoardModel
            {
                ID = boardID,
                SwimLanes = (from t in trelloSwimLanes
                             select new BoardModel.SwimLaneModel
                             {
                                 ID = t.ID,
                                 Name = t.Name,
                                 Cards = (from c in t.Cards
                                          select new BoardModel.CardModel
                                          {
                                              ID = c.ID
                                          }).ToArray()
                             }).ToArray(),
            };
        }
    }
}
