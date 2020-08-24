using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Hub.Hubs.Abstractions.Models;

namespace Kleinrechner.SplishSplash.Hub.Hubs.Abstractions
{
    public interface IPresenceTracker
    {
        Task<ConnectionOpenedResult> ConnectionOpened(string userId);

        Task<ConnectionClosedResult> ConnectionClosed(string userId);

        Task<string[]> GetOnlineUsers();
    }
}
