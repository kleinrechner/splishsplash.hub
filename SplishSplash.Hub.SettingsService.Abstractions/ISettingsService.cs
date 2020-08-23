using System;
using System.Collections.Generic;
using System.Text;
using Kleinrechner.SplishSplash.Backend.Authentication.Abstractions;

namespace Kleinrechner.SplishSplash.Hub.SettingsService.Abstractions
{
    public interface ISettingsService
    {
        void Save(AuthenticationSettings value);
    }
}
