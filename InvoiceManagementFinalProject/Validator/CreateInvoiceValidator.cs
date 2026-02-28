using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;
using FluentValidation;

namespace InvoiceManagementFinalProject.Validator;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.CustomerId)
           .NotEmpty().WithMessage("CustomerId is required");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be greater than StartDate");

        RuleFor(x => x.Comment)
               .MaximumLength(500).WithMessage("Comment must be at least 500 characters long")
               .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}
