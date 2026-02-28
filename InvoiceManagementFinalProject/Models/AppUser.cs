using Microsoft.AspNetCore.Identity;

namespace InvoiceManagementFinalProject.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; } 
    public string Address { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

}
