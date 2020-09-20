using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Kleinrechner.SplishSplash.Hub.Authentication.Abstractions;
using Kleinrechner.SplishSplash.Hub.Models;

namespace Kleinrechner.SplishSplash.Hub.Validator
{
    public class UpdateLoginUserValidator : AbstractValidator<UpdateLoginUser>
    {
        public UpdateLoginUserValidator()
        {
            RuleFor(x => x.Role).NotEmpty().IsEnumName(typeof(LoginUserRoles));
        }
    }
}
