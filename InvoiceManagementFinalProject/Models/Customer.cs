namespace InvoiceManagementFinalProject.Models;

public class Customer
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public AppUser? AppUser { get; set; }
    public IEnumerable<Invoice> Invoices { get; set; } = new List<Invoice>();
}

