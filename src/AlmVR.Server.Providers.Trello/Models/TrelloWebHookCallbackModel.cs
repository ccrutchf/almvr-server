using System;
using System.Collections.Generic;
using System.Text;

namespace AlmVR.Server.Providers.Trello.Models
{
    public class TrelloWebHookCallbackModel<T>
    {
        public T Model { get; set; }
    }
}
