using FluentValidation;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

namespace InvoiceManagementFinalProject.Validator;

public class UpdateInvoiceRowValidator : AbstractValidator<UpdateInvoiceRowRequest>
{
    public UpdateInvoiceRowValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}
