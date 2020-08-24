using System;
using System.Collections.Generic;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Hubs.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kleinrechner.SplishSplash.Hub.Hubs.Infrastructure
{
    public class Startup
    {
        #region Fields
        #endregion

        #region Ctor
        #endregion

        #region Methods

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPresenceTracker, PresenceTracker>();
        }

        #endregion
    }
}
