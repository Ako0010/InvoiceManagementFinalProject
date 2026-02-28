using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.Data;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;
using InvoiceManagementFinalProject.Models;
using InvoiceManagementFinalProject.Services.Interfaces;
using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System;

namespace InvoiceManagementFinalProject.Services;

public class InvoiceService : IInvoiceService
{
    private readonly HWDbContext _context;
    private readonly IMapper _mapper;
    public InvoiceService(HWDbContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<InvoiceResponseDto?> ArchiveInvoiceAsync(Guid id)
    {

        var invoice = await _context
                                .Invoices
                                .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (invoice is null) return null;



        invoice.DeletedAt = DateTimeOffset.UtcNow;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto?> ChangeInvoiceStatusAsync(Guid id, ChangeStatusInvoiceRequest changeStatusInvoiceRequest)
    {

        var invoice = await _context
                                .Invoices
                                .Include(i => i.Customer)
                                .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (invoice.Status != InvoiceStatus.Created)
            return null;


        invoice.Status = changeStatusInvoiceRequest.Status;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequest createInvoiceRequest)
    {
        var customer = await _context.Customers
                             .FirstOrDefaultAsync(c => c.Id == createInvoiceRequest.CustomerId);

        var invoice = _mapper.Map<Invoice>(createInvoiceRequest);

        invoice.CreatedAt = DateTimeOffset.UtcNow;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;


        invoice.InvoiceRows ??= new List<InvoiceRow>();

        foreach (var row in invoice.InvoiceRows)
        {
            row.Id = Guid.NewGuid();
            row.InvoiceId = invoice.Id;
            row.Sum = row.Quantity * row.Amount;
        }

        invoice.TotalSum = invoice.InvoiceRows.Sum(r => r.Sum);

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var result = await _context.Invoices
                           .Include(i => i.Customer)
                           .Include(i => i.InvoiceRows)
                           .FirstOrDefaultAsync(i => i.Id == invoice.Id);

        return _mapper.Map<InvoiceResponseDto>(result);
    }


    public async Task<bool> DeleteInvoiceAsync(Guid id)
    {
        var invoice = await _context
                            .Invoices
                            .FirstOrDefaultAsync(i => i.Id == id && i.DeletedAt == null);

        if (invoice == null) return false;

        if (invoice.Status != InvoiceStatus.Created) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync()
    {
        var invoices = await _context
                             .Invoices
                             .Where(i => i.DeletedAt == null)
                             .Include(t => t.Customer)
                             .Include(t => t.InvoiceRows)
                             .ToListAsync();

        return _mapper.Map<IEnumerable<InvoiceResponseDto>>(invoices);
    }

    public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _context
                            .Invoices
                            .Include(t => t.Customer)
                            .FirstOrDefaultAsync(t => t.Id == id);

        return _mapper.Map<InvoiceResponseDto?>(invoice);
    }

    public async Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(InvoiceQueryParams invoiceQueryParams)
    {
        invoiceQueryParams.Validate();

        var query = _context.Invoices
                            .Where(i => i.DeletedAt == null)
                            .Include(i => i.Customer)
                            .Include(i => i.InvoiceRows)
                            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(invoiceQueryParams.Search))
        {
            var searchTerm = invoiceQueryParams.Search.ToLower();

            query = query.Where(i =>
                i.Customer.Name.ToLower().Contains(searchTerm) ||
                i.Comment.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(invoiceQueryParams.Sort))
            query = ApplySorting(query, invoiceQueryParams.Sort, invoiceQueryParams.SortDirection);
        else
            query = query.OrderBy(i => i.Id);

        var totalCount = await query.CountAsync();

        var skip = (invoiceQueryParams.Page - 1) * invoiceQueryParams.PageSize;

        var invoices = await query
                                .Skip(skip)
                                .Take(invoiceQueryParams.PageSize)
                                .ToListAsync();

        var invoiceDtos = _mapper.Map<List<InvoiceResponseDto>>(invoices);

        return PagedResult<InvoiceResponseDto>.Create(
            invoiceDtos, 
            invoiceQueryParams.Page, 
            invoiceQueryParams.PageSize, 
            totalCount
            );
    }
    private IQueryable<Invoice> ApplySorting(IQueryable<Invoice> query, string sort, string sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";
        return sort.ToLower() switch
        {
            "customerid" => isDescending
               ? query.OrderByDescending(i => i.CustomerId)
               : query.OrderBy(i => i.CustomerId),
            "startdate" => isDescending
                ? query.OrderByDescending(i => i.StartDate)
                : query.OrderBy(i => i.StartDate),
            "enddate" => isDescending
                ? query.OrderByDescending(i => i.EndDate)
                : query.OrderBy(i => i.EndDate),
            "totalsum" => isDescending
                ? query.OrderByDescending(i => i.TotalSum)
                : query.OrderBy(i => i.TotalSum),
            "status" => isDescending
                ? query.OrderByDescending(i => i.Status)
                : query.OrderBy(i => i.Status),
            "createdat" => isDescending
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt),
            "updatedat" => isDescending
                ? query.OrderByDescending(i => i.UpdatedAt)
                : query.OrderBy(i => i.UpdatedAt),
            _ => query.OrderByDescending(i => i.CreatedAt)
        };
    }
        
    public async Task<InvoiceResponseDto?> UpdateInvoiceAsync(Guid id, UpdateInvoiceRequest updateInvoiceRequest)
    {
        var updatedInvoice = await _context
                            .Invoices
                            .Include(i => i.Customer)
                            .Include(i => i.InvoiceRows)
                            .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

        if (updatedInvoice == null) return null;

        if (updatedInvoice.Status != InvoiceStatus.Created)
            return null;


        _mapper.Map(updateInvoiceRequest, updatedInvoice);

        updatedInvoice.StartDate = updateInvoiceRequest.StartDate;
        updatedInvoice.EndDate = updateInvoiceRequest.EndDate;
        updatedInvoice.Comment = updateInvoiceRequest.Comment;
        updatedInvoice.UpdatedAt = DateTimeOffset.UtcNow;


        updatedInvoice.InvoiceRows = _mapper.Map<List<InvoiceRow>>(updateInvoiceRequest.Rows);

        foreach (var row in updatedInvoice.InvoiceRows)
        {
            row.Sum = row.Quantity * row.Amount;
        }

        updatedInvoice.TotalSum = updatedInvoice.InvoiceRows?.Sum(r => r.Sum) ?? 0;

        _context.Entry(updatedInvoice)
               .Property(x => x.TotalSum);


        await _context.SaveChangesAsync();
        return _mapper.Map<InvoiceResponseDto>(updatedInvoice);
    }
}
