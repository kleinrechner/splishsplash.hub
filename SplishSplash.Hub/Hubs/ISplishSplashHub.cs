using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplishSplash.Hub.Hubs
{
    public interface ISplishSplashHub
    {
        Task FrontendConnectedAsync();

        Task BackendConnectedAsync(string settingsJson);

        Task ClearAsync();
    }
}
