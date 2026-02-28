
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;
using FluentValidation;

namespace InvoiceManagementFinalProject.Validator;
public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Customer Name is required")
           .MinimumLength(3).WithMessage("Customer Name must be at least 3 characters long")
           .MaximumLength(100).WithMessage("Customer Name must not exceed 100 characters")
           .Matches(@"^[a-zA-Z\s]+$").WithMessage("Customer Name must only contain letters and spaces");


        RuleFor(x => x.Address)
              .NotEmpty().WithMessage("Customer Address is required")
              .MinimumLength(5).WithMessage("Customer Address must be at least 5 characters long")
              .MaximumLength(500).WithMessage("Customer Address must not exceed 500 characters");

        RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Customer Email is required")
              .EmailAddress().WithMessage("Customer Email must be a valid email address")
              .MaximumLength(255).WithMessage("Customer Email must not exceed 150 characters");

        RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Customer Phone Number is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Customer Phone Number must be in a valid international format");
    }
}
