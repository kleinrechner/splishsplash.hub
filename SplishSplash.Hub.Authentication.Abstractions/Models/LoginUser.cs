using System;
using System.Collections.Generic;
using System.Text;

namespace Kleinrechner.SplishSplash.Hub.Authentication.Abstractions.Models
{
    public class LoginUser
    {
        #region Fields

        public string DisplayName { get; set; }
        
        public string LoginName { get; set; }

        public string PasswordMD5Hash { get; set; }

        public string Role { get; set; }

        #endregion

        #region Ctor
        #endregion

        #region Methods
        #endregion
    }
}
