using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Hub.Hubs.Abstractions;
using Kleinrechner.SplishSplash.Hub.Hubs.Abstractions.Models;

namespace Kleinrechner.SplishSplash.Hub.Hubs
{
    public class PresenceTracker : IPresenceTracker
    {
        #region Fields

        private static readonly Dictionary<string, int> onlineUsers = new Dictionary<string, int>();

        #endregion

        #region Ctor
        #endregion

        #region Methods

        public Task<ConnectionOpenedResult> ConnectionOpened(string userId)
        {
            var joined = false;
            lock (onlineUsers)
            {
                if (onlineUsers.ContainsKey(userId))
                {
                    onlineUsers[userId] += 1;
                }
                else
                {
                    onlineUsers.Add(userId, 1);
                    joined = true;
                }
            }
            return Task.FromResult(new ConnectionOpenedResult { UserJoined = joined });
        }

        public Task<ConnectionClosedResult> ConnectionClosed(string userId)
        {
            var left = false;
            lock (onlineUsers)
            {
                if (onlineUsers.ContainsKey(userId))
                {
                    onlineUsers[userId] -= 1;
                    if (onlineUsers[userId] <= 0)
                    {
                        onlineUsers.Remove(userId);
                        left = true;
                    }
                }
            }

            return Task.FromResult(new ConnectionClosedResult { UserLeft = left });
        }

        public Task<string[]> GetOnlineUsers()
        {
            lock (onlineUsers)
            {
                return Task.FromResult(onlineUsers.Keys.ToArray());
            }
        }

        #endregion
    }
}
