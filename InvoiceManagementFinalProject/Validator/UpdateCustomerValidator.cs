using InvoiceManagementFinalProject.DTOs.Customer_DTOs;
using FluentValidation;

namespace InvoiceManagementFinalProject.Validator;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Customer Name is required")
           .MinimumLength(3).WithMessage("Customer Name must be at least 3 characters long");

        RuleFor(x => x.Address)
              .NotEmpty().WithMessage("Customer Address is required")
              .MinimumLength(5).WithMessage("Customer Address must be at least 5 characters long");

        RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Customer Email is required")
               .EmailAddress().WithMessage("Customer Email must be a valid email address");
    }
}
