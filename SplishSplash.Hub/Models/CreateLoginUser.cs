using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kleinrechner.SplishSplash.Hub.Models
{
    public class CreateLoginUser
    {
        public string LoginName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}
