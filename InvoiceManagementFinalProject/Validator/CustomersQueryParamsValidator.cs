using FluentValidation;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;

namespace InvoiceManagementFinalProject.Validator;

public class CustomersQueryParamsValidator : AbstractValidator<CustomerQueryParams>
{
    public CustomersQueryParamsValidator() 
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
            .Must(x => x == null || new[] { "name", "email", "createdat" }
            .Contains(x.ToLower()))
            .WithMessage("Invalid sort field. Valid Values: Name  , Email , CreatedAt  ")
            .When(x => !string.IsNullOrWhiteSpace(x.Sort));

        RuleFor(x => x.Search)
            .MaximumLength(50).WithMessage("Search term is too long.")
            .When(x => !string.IsNullOrEmpty(x.Search));
    }
}
