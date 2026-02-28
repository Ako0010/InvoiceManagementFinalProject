namespace InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

public class CreateInvoiceRequest
{
    public Guid? CustomerId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Comment { get; set; }
    public List<CreateInvoiceRowDto> Rows { get; set; } = new();

}
