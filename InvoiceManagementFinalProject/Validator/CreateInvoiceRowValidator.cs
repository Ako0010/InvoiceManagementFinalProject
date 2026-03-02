using FluentValidation;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

namespace InvoiceManagementFinalProject.Validator;

public class CreateInvoiceRowValidator : AbstractValidator<CreateInvoiceRowDto>
{
    public CreateInvoiceRowValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Amount)
            .GreaterThan(0);

    }
}
