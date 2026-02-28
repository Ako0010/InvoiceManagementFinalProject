namespace InvoiceManagementFinalProject.DTOs;

public class AttachmentResponseDto
{
    public int Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string UploadedUserId { get; set; } = string.Empty;
    public DateTimeOffset UploadedAt { get; set; }
}