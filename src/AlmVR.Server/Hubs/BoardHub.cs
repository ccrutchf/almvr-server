using AlmVR.Common.Models;
using AlmVR.Server.Core.Providers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AlmVR.Server.Hubs
{
    public class BoardHub : Hub
    {
        private IBoardProvider boardProvider;

        public BoardHub(IBoardProvider boardProvider)
        {
            this.boardProvider = boardProvider;
        }

        public Task<BoardModel> GetBoard()
        {
            return boardProvider.GetBoardAsync();
        }
    }
}
