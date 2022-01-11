using CookieAuthentication.Models;
using FluentValidation;

namespace CookieAuthentication.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(model => model.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} Field Required!")
                .MaximumLength(8).WithMessage("{PropertyName} Shouldn`t Exceed 8 Letters");

            RuleFor(model => model.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} Field Required!")
                .Length(8, 50).WithMessage("{PropertyName} Should be Between 8 and 50 Letters");


        }
    }
}
