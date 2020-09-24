using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models;
using Newtonsoft.Json;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Extensions
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

        public static List<Claim> GetClaims(this LoginUser loginUser)
        {
            var loginUserHash = loginUser.GetMD5Hash();
            var claimList = new List<Claim>(new[] {
                new Claim(ClaimTypes.NameIdentifier, loginUser.LoginName.ToLower()),
                new Claim(ClaimTypes.Name, loginUser.LoginName.ToLower()),
                new Claim(ClaimTypes.Role, loginUser.Role),
                new Claim(ClaimTypes.Hash, loginUserHash) 
            });

            return claimList;
        }

        public static string GetMD5Hash(this LoginUser loginUser)
        {
            return JsonConvert.SerializeObject(loginUser.WithoutPassword()).GetMD5Hash();
        }
    }
}
