using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Backend.HubClientBackgroundService.Abstractions;
using Kleinrechner.SplishSplash.Backend.HubClientBackgroundService.Abstractions.Models;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.Hubs.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kleinrechner.SplishSplash.Hub.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer, BasicAuthentication")]
    public class SplishSplashHub : Hub<ISplishSplashHubClient>, ISplishSplashHub
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

        [Authorize(Roles = nameof(LoginUserRoles.Backend))]
        public async Task ConnectBackend(SettingsHubModel settingsHubModel)
        {
            FillHubModel(settingsHubModel);

            if (!string.IsNullOrWhiteSpace(settingsHubModel.ReceiverUserName))
            {
                await Clients.User(settingsHubModel.ReceiverUserName).BackendConnected(settingsHubModel);
            }
            else
            {
                await Clients.Groups(nameof(LoginUserRoles.Frontend)).BackendConnected(settingsHubModel);
            }
        }

        [Authorize(Roles = nameof(LoginUserRoles.Backend))]
        public async Task SendGpioPinChanged(GpioPinChangedModel gpioPinChangedModel)
        {
            FillHubModel(gpioPinChangedModel);
            await Clients.Groups(nameof(LoginUserRoles.Frontend)).GpioPinChangedReceived(gpioPinChangedModel);
        }

        [Authorize(Roles = nameof(LoginUserRoles.Frontend))]
        public async Task SendUpdateSettings(SettingsHubModel settingsHubModel)
        {
            FillHubModel(settingsHubModel);
            await Clients.User(settingsHubModel.ReceiverUserName).UpdateSettingsReceived(settingsHubModel);
        }

        [Authorize(Roles = nameof(LoginUserRoles.Frontend))]
        public async Task SendChangeGpioPin(ChangeGpioPinModel changeGpioPinModel)
        {
            FillHubModel(changeGpioPinModel);
            await Clients.User(changeGpioPinModel.ReceiverUserName).ChangeGpioPinReceived(changeGpioPinModel);
        }

        public override async Task OnConnectedAsync()
        {
            var roleName = GetRoleName();
            var result = await _presenceTracker.ConnectionOpened(GetUserName());
            if (result.UserJoined)
            {
                if (roleName == nameof(LoginUserRoles.Frontend))
                {
                    var hubModel = new BaseHubModel();
                    FillHubModel(hubModel);
                    await Clients.Groups(nameof(LoginUserRoles.Backend)).FrontendConntected(hubModel);
                }
            }

            if (roleName == nameof(LoginUserRoles.Backend))
            {
                var hubModel = new BaseHubModel();
                await Clients.Caller.FrontendConntected(hubModel);
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roleName);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var roleName = GetRoleName();
            var result = await _presenceTracker.ConnectionClosed(GetUserName());
            if (result.UserLeft)
            {
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    if (roleName == nameof(LoginUserRoles.Backend))
                    {
                        var hubModel = new BaseHubModel();
                        FillHubModel(hubModel);
                        await Clients.Groups(nameof(LoginUserRoles.Frontend)).BackendDisconnected(hubModel);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roleName);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private void FillHubModel(BaseHubModel baseHubModel)
        {
            baseHubModel.SenderUserName = GetUserName();
        }

        private string GetUserName()
        {
            return Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        
        private string GetRoleName()
        {
            return Context.User.FindFirst(ClaimTypes.Role)?.Value;
        }

        #endregion
    }
}
