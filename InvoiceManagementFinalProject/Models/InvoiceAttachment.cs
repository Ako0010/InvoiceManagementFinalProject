namespace InvoiceManagementFinalProject.Models;

public class InvoiceAttachment
{
    public int Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }

    public string UploadedUserId { get; set; } = string.Empty;
    public AppUser UploadedUser { get; set; } = null!;
    public DateTimeOffset UploadedAt { get; set; }

}
