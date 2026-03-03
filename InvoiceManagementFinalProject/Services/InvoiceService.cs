using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.Data;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;
using InvoiceManagementFinalProject.Models;
using InvoiceManagementFinalProject.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Xceed.Document.NET;
using Xceed.Drawing;
using Xceed.Words.NET;
using Azure;

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

        if (Enum.TryParse<InvoiceStatus>(changeStatusInvoiceRequest.Status, out var status))
        {
          invoice.Status = status;
        }

        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequest createInvoiceRequest, string currentUserId)
    {
        var customer = await _context.Customers
                             .FirstOrDefaultAsync(c => c.Id == createInvoiceRequest.CustomerId && c.UserId == currentUserId);

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

    public async Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync(string currentUserId)
    {
        var invoices = await _context
                             .Invoices
                             .Where(i => i.DeletedAt == null && i.Customer.UserId == currentUserId)
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

        if (!string.IsNullOrEmpty(invoiceQueryParams.CustomerName))
        {
            var searchTerm = invoiceQueryParams.CustomerName.ToLower();
            query = query.Where(i => i.Customer!.Name.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(invoiceQueryParams.Status))
        {
            if (Enum.TryParse<InvoiceStatus>(invoiceQueryParams.Status, true, out var status))
            {
                query = query.Where(i => i.Status == status);
            }
        }

        if (invoiceQueryParams.MinTotal.HasValue)
        {
            query = query.Where(i => i.TotalSum >= invoiceQueryParams.MinTotal.Value);
        }

        if (invoiceQueryParams.MaxTotal.HasValue)
        {
            query = query.Where(i => i.TotalSum <= invoiceQueryParams.MaxTotal.Value);
        }

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

    public async Task<(byte[] Content, string FileName, string ContentType)?> DownloadInvoiceAsync(Guid id, string format)
    {
        var invoice = _context.Invoices
           .Include(i => i.Customer)
           .Include(i => i.InvoiceRows)
           .FirstOrDefault(i => i.Id == id && i.DeletedAt == null);

        if (invoice == null) return null;

        if (format?.ToLower() == "docx")
        {
            var content = GenerateDocX(invoice);
            return (content, $"Invoice_{invoice.Id}.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
        else
        {
            var content = GeneratePdf(invoice);
            return (content, $"Invoice_{invoice.Id}.pdf", "application/pdf");
        }
    }

    private byte[] GeneratePdf(Invoice invoice)
    {
        return QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("INVOICE").FontSize(20).SemiBold();
                        col.Item().Text($"Invoice ID: {invoice.Id}").FontSize(9);
                        col.Item().Text($"Status: {invoice.Status}").FontSize(9);
                    });

                    row.ConstantItem(220).AlignRight().Column(col =>
                    {
                        col.Item().Text("Invoice Management").SemiBold();
                        col.Item().Text($"Created: {invoice.CreatedAt:yyyy-MM-dd}").FontSize(9);
                        col.Item().Text($"Updated: {invoice.UpdatedAt:yyyy-MM-dd}").FontSize(9);
                    });
                });

                page.Content().Column(col =>
                {
                    col.Spacing(14);

                    col.Item().Border(1).Padding(10).Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text("Bill To").SemiBold();
                            left.Item().Text(invoice.Customer?.Name ?? "N/A");
                        });

                        row.ConstantItem(220).Column(right =>
                        {
                            right.Item().Text("Period").SemiBold();
                            right.Item().Text($"Start: {invoice.StartDate:yyyy-MM-dd}");
                            right.Item().Text($"End: {invoice.EndDate:yyyy-MM-dd}");
                        });
                    });

                    if (!string.IsNullOrWhiteSpace(invoice.Comment))
                    {
                        col.Item().Text("Comment").SemiBold();
                        col.Item().Text(invoice.Comment);
                    }

                    col.Item().Text("Items").SemiBold();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(6);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Border(1).Padding(5)
                                .Text("Service").SemiBold();

                            header.Cell().Background(Colors.Grey.Lighten3).Border(1).Padding(5)
                                .AlignRight().Text("Qty").SemiBold();

                            header.Cell().Background(Colors.Grey.Lighten3).Border(1).Padding(5)
                                .AlignRight().Text("Amount").SemiBold();

                            header.Cell().Background(Colors.Grey.Lighten3).Border(1).Padding(5)
                                .AlignRight().Text("Sum").SemiBold();
                        });

                        foreach (var r in invoice.InvoiceRows ?? new List<InvoiceRow>())
                        {
                            table.Cell().Border(1).Padding(5)
                                .Text(r.Service);

                            table.Cell().Border(1).Padding(5)
                                .AlignRight().Text(r.Quantity.ToString());

                            table.Cell().Border(1).Padding(5)
                                .AlignRight().Text(r.Amount.ToString("0.00"));

                            table.Cell().Border(1).Padding(5)
                                .AlignRight().Text(r.Sum.ToString("0.00"));
                        }
                    });

                    col.Item().AlignRight().Border(1).Padding(10).Row(r =>
                    {
                        r.RelativeItem().AlignRight().Text("Total:").SemiBold();
                        r.ConstantItem(120).AlignRight().Text(invoice.TotalSum.ToString("0.00")).SemiBold();
                    });
                });

                page.Footer().AlignCenter()
                    .Text("Thank you for your business • Invoice Management System");
            });
        }).GeneratePdf();
    }

    private byte[] GenerateDocX(Invoice invoice)
    {
        using var ms = new MemoryStream();
        using var doc = DocX.Create(ms);

        doc.InsertParagraph("INVOICE").FontSize(22).Bold().SpacingAfter(10);

        doc.InsertParagraph($"Invoice ID: {invoice.Id}").FontSize(10);
        doc.InsertParagraph($"Status: {invoice.Status}").FontSize(10);
        doc.InsertParagraph($"Created: {invoice.CreatedAt:yyyy-MM-dd}").FontSize(10);
        doc.InsertParagraph($"Updated: {invoice.UpdatedAt:yyyy-MM-dd}").FontSize(10);

        doc.InsertParagraph().SpacingAfter(10);

        doc.InsertParagraph("Bill To").Bold().FontSize(12).SpacingAfter(4);
        doc.InsertParagraph(invoice.Customer?.Name ?? "N/A").FontSize(11);

        doc.InsertParagraph().SpacingAfter(6);

        doc.InsertParagraph("Period").Bold().FontSize(12).SpacingAfter(4);
        doc.InsertParagraph($"Start: {invoice.StartDate:yyyy-MM-dd}").FontSize(11);
        doc.InsertParagraph($"End: {invoice.EndDate:yyyy-MM-dd}").FontSize(11);

        doc.InsertParagraph().SpacingAfter(10);

        if (!string.IsNullOrWhiteSpace(invoice.Comment))
        {
            doc.InsertParagraph("Comment").Bold().FontSize(12).SpacingAfter(4);
            doc.InsertParagraph(invoice.Comment).FontSize(11);
            doc.InsertParagraph().SpacingAfter(10);
        }

        doc.InsertParagraph("Items").Bold().FontSize(12).SpacingAfter(6);

        var rows = invoice.InvoiceRows ?? new List<InvoiceRow>();
        var table = doc.AddTable(rows.Count + 1, 4);
        table.Design = TableDesign.TableGrid;

        table.Rows[0].Cells[0].Paragraphs[0].Append("Service").Bold();
        table.Rows[0].Cells[1].Paragraphs[0].Append("Qty").Bold();
        table.Rows[0].Cells[2].Paragraphs[0].Append("Amount").Bold();
        table.Rows[0].Cells[3].Paragraphs[0].Append("Sum").Bold();

        for (int i = 0; i < rows.Count; i++)
        {
            var r = rows[i];
            var tr = table.Rows[i + 1];

            tr.Cells[0].Paragraphs[0].Append(r.Service ?? "");
            tr.Cells[1].Paragraphs[0].Append(r.Quantity.ToString()).Alignment = Alignment.right;
            tr.Cells[2].Paragraphs[0].Append(r.Amount.ToString("0.00")).Alignment = Alignment.right;
            tr.Cells[3].Paragraphs[0].Append(r.Sum.ToString("0.00")).Alignment = Alignment.right;
        }

        doc.InsertTable(table);
        doc.InsertParagraph().SpacingAfter(10);

        var totalP = doc.InsertParagraph();
        totalP.Append("Total: ").Bold();
        totalP.Append(invoice.TotalSum.ToString("0.00")).Bold();
        totalP.Alignment = Alignment.right;

        doc.Save();
        return ms.ToArray();
    }

}
