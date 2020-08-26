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
    public class CreateLoginUserValidator : AbstractValidator<CreateLoginUser>
    {
        public CreateLoginUserValidator(IOptions<AuthenticationSettings> _authenticationSettings)
        {
            RuleFor(x => x.LoginName)
                    .NotEmpty()
                    .Must(x => !_authenticationSettings.Value.Users.Select(x => x.LoginName).Contains(x))
                    .WithMessage("LoginName already exist");

            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Role).NotEmpty().IsEnumName(typeof(LoginUserRoles));
        }
    }
}
