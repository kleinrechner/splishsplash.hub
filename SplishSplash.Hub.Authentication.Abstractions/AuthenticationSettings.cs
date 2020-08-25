using System;
using System.Collections.Generic;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Abstractions
{
    public class AuthenticationSettings
    {
        public const string SectionName = "AuthenticationSettings";

        public List<LoginUser> Users { get; set; }
    }
}
