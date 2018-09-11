using AlmVR.Common.Models;
using AlmVR.Server.Core;
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
            this.cardProvider.CardChanged += CardProvider_CardChanged;
        }

        public Task<CardModel> GetCard(string id)
        {
            return cardProvider.GetCardAsync(id);
        }

        public Task MoveCard(CardModel card, SwimLaneModel targetSwimLane)
        {
            return cardProvider.MoveCardAsync(card, targetSwimLane);
        }

        protected override void Dispose(bool disposing)
        {
            // Prevent memory leak.
            cardProvider.CardChanged -= CardProvider_CardChanged;

            base.Dispose(disposing);
        }

        private async void CardProvider_CardChanged(object sender, CardChangedEventArgs e)
        {
            await this.Clients.All.SendAsync("CardChanged", e.Card);
        }
    }
}
