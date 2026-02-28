namespace InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

public class InvoiceRowResponseDto
{
    public Guid Id { get; set; }
    public string Service { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal Sum { get; set; }
}
