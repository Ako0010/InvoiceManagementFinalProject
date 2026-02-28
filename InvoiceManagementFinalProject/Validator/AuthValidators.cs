using FluentValidation;
using InvoiceManagementFinalProject.DTOs.User_DTOs;

namespace InvoiceManagementFinalProject.Validator;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Firstname must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Passwords must be at least 6 characters.")
            .WithMessage("Passwords must have at least one digit ('0'-'9').,Passwords must have at least one lowercase ('a'-'z').,Passwords must have at least one uppercase ('A'-'Z')");

    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Passwords must be at least 6 characters.")
                .WithMessage("Passwords must have at least one digit ('0'-'9').,Passwords must have at least one lowercase ('a'-'z').,Passwords must have at least one uppercase ('A'-'Z')");
    }

}