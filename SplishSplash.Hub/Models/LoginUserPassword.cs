using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kleinrechner.SplishSplash.Hub.Models
{
    public class LoginUserPassword
    {
        [Required]
        public string Password { get; set; }
    }
}
