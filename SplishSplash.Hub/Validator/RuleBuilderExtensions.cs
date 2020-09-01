using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Kleinrechner.SplishSplash.Hub.Validator
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> LoginName<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 3, int maxLength = 50)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("LoginName can not be empty")
                .MinimumLength(minimumLength).WithMessage($"LoginName must have at least {minimumLength} characters")
                .MaximumLength(maxLength).WithMessage($"LoginName must have up to {maxLength} characters")
                .Matches(@"[^a-zA-Z0-9_\-@#\.]").WithMessage("The LoginName can only contain to following characters [a-zA-Z0-9_-@#.]");
            return options;
        }

        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength = 16)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage("Password can not be empty")
                .MinimumLength(minimumLength).WithMessage($"Password must have at least {minimumLength} characters")
                .Matches("[A-Z]").WithMessage("Password must contain upper case letter")
                .Matches("[a-z]").WithMessage("Password must contain lower case letter")
                .Matches("[0-9]").WithMessage("Password must contain digits")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain special charaters");
            return options;
        }
    }
}
