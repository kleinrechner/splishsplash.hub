using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kleinrechner.SplishSplash.Backend.Authentication.Abstractions;

namespace Kleinrechner.SplishSplash.Hub.Extensions
{
    public static class LoginUserExtensions
    {
        public static LoginUser WithoutPassword(this LoginUser loginUser)
        {
            return new LoginUser()
            {
                LoginName = loginUser.LoginName,
                Role = loginUser.Role
            };
        }
    }
}
