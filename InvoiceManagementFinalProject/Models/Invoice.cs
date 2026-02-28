namespace InvoiceManagementFinalProject.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public List<InvoiceRow> InvoiceRows { get; set; } = new();
    public decimal TotalSum { get; set; }
    public string? Comment { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Created;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<InvoiceAttachment> Attachments { get; set; } = new List<InvoiceAttachment>();
}

public enum InvoiceStatus
{
    Created,
    Sent,
    Received,
    Paid,
    Cancelled,
    Rejected
}