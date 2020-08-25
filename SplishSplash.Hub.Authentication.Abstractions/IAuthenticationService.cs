using System;
using System.Collections.Generic;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Abstractions
{
    public interface IAuthenticationService
    {
        List<LoginUser> GetLoginUsers();
    }
}
