using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.Models;
using Microsoft.Extensions.Options;

namespace Kleinrechner.SplishSplash.Hub.Validator
{
    public class LoginUserPasswordValidator : AbstractValidator<LoginUserPassword>
    {
        public LoginUserPasswordValidator()
        {
            RuleFor(x => x.Password).Password();
        }
    }
}
