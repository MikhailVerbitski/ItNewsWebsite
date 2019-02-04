using Domain.Contracts.Models.ViewModels.Account;
using FluentValidation;

namespace Domain.Contracts.Validators.ViewModels.Account
{
    public class RegisterValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterValidator()
        {
            RuleFor(a => a.FirstName).NotEmpty();
            RuleFor(a => a.LastName).NotEmpty();
            RuleFor(a => a.Login).NotEmpty();
            RuleFor(a => a.Password).NotEmpty().Length(6,20);
            RuleFor(a => a.Email).NotEmpty().EmailAddress();
        }
    }
}
