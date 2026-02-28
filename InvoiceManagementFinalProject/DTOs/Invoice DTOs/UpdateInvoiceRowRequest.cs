namespace InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

public class UpdateInvoiceRowRequest
{
    public string Service { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
}
