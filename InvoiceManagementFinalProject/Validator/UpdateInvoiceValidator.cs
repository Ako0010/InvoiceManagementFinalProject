using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;
using FluentValidation;

namespace InvoiceManagementFinalProject.Validator;

public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceRequest>
{
    public UpdateInvoiceValidator()
    {
        RuleFor(x => x)
           .Must(x => x.CustomerId != default||
                      x.StartDate != default ||
                      x.EndDate != default ||
                      !string.IsNullOrWhiteSpace(x.Comment))
           .WithMessage("At least one field (CustomerId, StartDate, EndDate, Comment) must be provided for update.");



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

        RuleForEach(x => x.Rows)
            .SetValidator(new UpdateInvoiceRowValidator());
    }
}
