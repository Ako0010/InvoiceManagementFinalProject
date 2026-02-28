namespace InvoiceManagementFinalProject.DTOs.Customer_DTOs;

public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; } 
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

}
