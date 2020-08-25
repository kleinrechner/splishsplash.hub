using System;
using System.Collections.Generic;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Extensions
{
    public static class LoginUserExtensions
    {
        public static LoginUser WithoutPassword(this LoginUser loginUser)
        {
            return new LoginUser()
            {
                DisplayName = loginUser.DisplayName,
                LoginName = loginUser.LoginName,
                Role = loginUser.Role
            };
        }
    }
}
