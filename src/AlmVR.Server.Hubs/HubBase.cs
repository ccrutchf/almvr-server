using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AlmVR.Server.Hubs
{
    public class HubBase : Hub
    {
        public void Ping()
        {
            Debugger.Log(0, "test", "ping");
        }
    }
}
