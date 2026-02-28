using InvoiceManagementFinalProject.Models;

namespace InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

public class ChangeStatusInvoiceRequest
{
    public InvoiceStatus Status { get; set; }
}
