using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.Data;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;
using InvoiceManagementFinalProject.Models;
using InvoiceManagementFinalProject.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagementFinalProject.Services;

public class CustomerService : ICustomerService
{
    private readonly HWDbContext _context;
    private readonly IMapper _mapper;
    public CustomerService(HWDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<CustomerResponseDto?> ArchiveCustomerAsync(Guid id)
    {

        var customer = await _context
                             .Customers
                             .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

        if (customer is null) return null;

        customer.DeletedAt = DateTimeOffset.UtcNow;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerRequest createCustomerRequest,string currentUserId)
    {
        var customer = _mapper.Map<Customer>(createCustomerRequest);

        customer.UserId = currentUserId;
        customer.CreatedAt = DateTimeOffset.UtcNow;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerResponseDto>(customer);
    }


    public async Task<bool> DeleteCustomerAsync(Guid id)
    {
        var customer = await _context
                                .Customers
                                .Include(c => c.Invoices)
                                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

        if (customer == null) return false; 

        var hasSentInvoices = customer.Invoices.Any(i => i.Status != InvoiceStatus.Created);

        if (hasSentInvoices) return false;


        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;

    }

    public async Task<List<CustomerResponseDto>> GetAllCustomersAsync(string currentUserId)
    {
        var customers = await _context
            .Customers
            .Where(c => c.DeletedAt == null && c.UserId == currentUserId)
            .ToListAsync();

        return _mapper.Map<List<CustomerResponseDto>>(customers);
    }

    public async Task<PagedResult<CustomerResponseDto>> GetPagedAsync(CustomerQueryParams customerQueryParams)
    {
        customerQueryParams.Validate();

        var query = _context.Customers
                             .Where(c => c.DeletedAt == null)
                             .AsQueryable();

        if (!string.IsNullOrWhiteSpace(customerQueryParams.Search))
        {
            var searchTerm = customerQueryParams.Search.ToLower();
            query = query.Where(c => 
            c.Name.ToLower().Contains(searchTerm) || 
            c.Email.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(customerQueryParams.Sort))
            query = ApplySorting(query, customerQueryParams.Sort, customerQueryParams.SortDirection);
        else
            query = query.OrderBy(c => c.Id);

        var totalCount = await query.CountAsync();

        var skip = (customerQueryParams.Page - 1) * customerQueryParams.PageSize;

        var customers = await query
            .Skip(skip)
            .Take(customerQueryParams.PageSize)
            .ToListAsync();

        var customerDtos = _mapper.Map<List<CustomerResponseDto>>(customers);

      return PagedResult<CustomerResponseDto>.Create(
          customerDtos, 
          customerQueryParams.Page, 
          customerQueryParams.PageSize, 
          totalCount
          );
    }

    private IQueryable<Customer> ApplySorting(IQueryable<Customer> query, string sort, string sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";
        return sort.ToLower() switch
        {
            "name" => isDescending
                                 ? query.OrderByDescending(t => t.Name)
                                 : query.OrderBy(t => t.Name),

            "email" => isDescending
                                ? query.OrderByDescending(t => t.Email)
                                : query.OrderBy(t => t.Email),

            "createdat" => isDescending
                                ? query.OrderByDescending(t => t.CreatedAt)
                                : query.OrderBy(t => t.CreatedAt),

            _ => query.OrderBy(t => t.Id)
        };
    }


    public async Task<CustomerResponseDto> GetCustomerByIdAsync(Guid customerId,string currentUserId)
    {
        var customer = await _context
                                .Customers
                                .Where(c => c.DeletedAt == null && c.UserId == currentUserId)                                
                                .FirstOrDefaultAsync(c => c.Id == customerId);
        if (customer is null)
            return null;

        return _mapper.Map<CustomerResponseDto>(customer);
    }

    public async Task<CustomerResponseDto> UpdateCustomerAsync(Guid id, UpdateCustomerRequest updateCustomerRequest)
    {

        var updatedCustomer = await _context
                                    .Customers
                                    .Include(c => c.Invoices)
                                    .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);

        _mapper.Map(updateCustomerRequest, updatedCustomer);
        updatedCustomer.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<CustomerResponseDto>(updatedCustomer);
    }
}

