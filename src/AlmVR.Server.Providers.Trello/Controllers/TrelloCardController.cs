﻿using AlmVR.Server.Providers.Trello.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AlmVR.Server.Providers.Trello.Controllers
{
    [Route("api/[controller]")]
    public class TrelloCardController : Controller
    {
        private readonly TrelloCardProvider trelloCardProvider;

        public TrelloCardController(TrelloCardProvider trelloCardProvider)
        {
            this.trelloCardProvider = trelloCardProvider;
        }

        [HttpHead]
        public IActionResult Head() => Ok();

        [HttpPost]
        public IActionResult Post([FromBody]TrelloWebHookCallbackModel<TrelloCardModel> callbackModel)
        {
            trelloCardProvider.NotifyClientsAsync(callbackModel.Model);

            return Ok();
        }
    }
}
