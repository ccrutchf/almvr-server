using AlmVR.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlmVR.Server.Providers.Trello.Models
{
    public class TrelloCardModel
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public CardModel ToCommonCardModel() =>
            new CardModel
            {
                ID = ID,
                Name = Name
            };
    }
}
