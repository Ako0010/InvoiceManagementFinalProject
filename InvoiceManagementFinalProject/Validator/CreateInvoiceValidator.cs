using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;
using FluentValidation;

namespace InvoiceManagementFinalProject.Validator;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceValidator()
    {

        RuleFor(x => x.CustomerId)
           .NotEmpty().WithMessage("CustomerId is required");

        RuleFor(x => x.Comment)
               .MinimumLength(3).WithMessage("Comment must be at least 3 characters long");

    }
}
