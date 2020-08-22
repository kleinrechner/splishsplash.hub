using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Backend.HubClientBackgroundService.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kleinrechner.SplishSplash.Hub.Hubs
{
    [Authorize]
    public class SplishSplashHub : Microsoft.AspNetCore.SignalR.Hub<ISplishSplashHubClient>
    {
        public Task SendMessage(string user, string message)
        {
            //return Clients.All.SendAsync("ReceiveMessage", user, message);
            return Task.CompletedTask;
        }
    }
}
