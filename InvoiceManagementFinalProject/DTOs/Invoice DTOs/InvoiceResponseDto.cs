using InvoiceManagementFinalProject.Models;

namespace InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public decimal TotalSum { get; set; }
    public string? Comment { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<InvoiceRowResponseDto> Rows { get; set; } = new List<InvoiceRowResponseDto>();
}
