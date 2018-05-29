using AlmVR.Common.Models;
using AlmVR.Server.Core.Providers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlmVR.Server.Hubs
{
    public class CardHub : Hub
    {
        private ICardProvider cardProvider;

        public CardHub(ICardProvider cardProvider)
        {
            this.cardProvider = cardProvider;
        }

        public Task<CardModel> GetCard(string id)
        {
            return cardProvider.GetCardAsync(id);
        }
    }
}
