using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Backend.HubClientBackgroundService.Abstractions;
using Kleinrechner.SplishSplash.Hub.Hubs.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kleinrechner.SplishSplash.Hub.Hubs
{
    [Authorize]
    public class SplishSplashHub : Microsoft.AspNetCore.SignalR.Hub<ISplishSplashHubClient>, ISplishSplashHub
    {
        #region Fields

        private readonly IPresenceTracker _presenceTracker;

        #endregion

        #region Ctor

        public SplishSplashHub(IPresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        #endregion

        #region Methods

        public override async Task OnConnectedAsync()
        {
            var result = await _presenceTracker.ConnectionOpened(Context.User.Identity.Name);
            if (result.UserJoined)
            {
                //await Clients.All.SendAsync("newMessage", "system", $"{Context.User.Identity.Name} joined");
            }

            var roleName = Context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roleName);
            }

            //await Clients.Caller.SendAsync("newMessage", "system", $"Currently online:\n{string.Join("\n", currentUsers)}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var result = await _presenceTracker.ConnectionClosed(Context.User.Identity.Name);
            if (result.UserLeft)
            {
                //await Clients.All.SendAsync("newMessage", "system", $"{Context.User.Identity.Name} left");
            }

            var roleName = Context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roleName);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public Task SendMessage(string user, string message)
        {
            //return Clients.All.SendAsync("ReceiveMessage", user, message);
            return Task.CompletedTask;
        }

        #endregion
    }
}
