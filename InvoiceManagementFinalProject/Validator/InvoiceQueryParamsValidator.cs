using FluentValidation;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

namespace InvoiceManagementFinalProject.Validator;

public class InvoiceQueryParamsValidator : AbstractValidator<InvoiceQueryParams>
{ 
    public InvoiceQueryParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SortDirection)
            .Must(x => x == null || x.ToLower() == "asc" || x.ToLower() == "desc")
            .WithMessage("Sort direction must be 'asc' or 'desc'")
            .When(x => !string.IsNullOrWhiteSpace(x.SortDirection));

        RuleFor(x => x.Sort)
            .Must(x => x == null || new[] { "customerid", "customername", "startdate", "enddate", "totalsum", "comment", "status", "createdat", "updatedat" }
            .Contains(x.ToLower()))
            .WithMessage("Invalid sort field. Valid Values: CustomerId , CustomerName , StartDate , EndDate, TotalSum , Commment , Status , CreatedAt , UpdatedAt ")
            .When(x => !string.IsNullOrWhiteSpace(x.Sort));

        RuleFor(x => x.Search)
            .MaximumLength(50).WithMessage("Search term is too long.")
            .When(x => !string.IsNullOrEmpty(x.Search));


    }
}
