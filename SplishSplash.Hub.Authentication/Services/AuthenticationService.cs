using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models;
using Microsoft.Extensions.Options;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly IOptions<AuthenticationSettings> _authenticationSettings;

        #endregion

        #region Ctor
        public AuthenticationService(IOptions<AuthenticationSettings> authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        #endregion

        #region Methods

        public virtual List<string> GetRoleNames()
        {
            return Enum.GetNames(typeof(LoginUserRoles)).ToList();
        }

        public virtual List<LoginUser> GetLoginUsers()
        {
            return _authenticationSettings.Value.Users;
        }

        #endregion
    }
}
