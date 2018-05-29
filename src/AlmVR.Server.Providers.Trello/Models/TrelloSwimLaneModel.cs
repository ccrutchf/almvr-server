using System;
using System.Collections.Generic;
using System.Text;

namespace AlmVR.Server.Providers.Trello.Models
{
    class TrelloSwimLaneModel
    {
        public class TrelloCardModel
        {
            public string ID { get; set; }
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<TrelloCardModel> Cards { get; set; }
    }
}
