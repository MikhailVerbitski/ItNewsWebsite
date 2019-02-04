using Domain.Contracts.Models.ViewModels.Account;
using FluentValidation;

namespace Domain.Contracts.Validators.ViewModels.Account
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            RuleFor(a => a.Login).NotEmpty();
            RuleFor(a => a.Password).NotEmpty().Length(6, 20);
        }
    }
}
