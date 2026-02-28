using InvoiceManagementFinalProject.DTOs;

namespace InvoiceManagementFinalProject.Services.Interfaces;

public interface IAttachmentService
{
    Task<AttachmentResponseDto?> UploadAsync(
        Guid invoiceId,
        Stream fileStream,
        string originalFileName,
        string contentType,
        long length,
        string userId,
        CancellationToken cancellationToken = default);


    Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(
        int attachmentId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default);

    Task<InvoiceAttachmentInfo?> GetAttachmentInfoAsync(int attachmentId, CancellationToken cancellationToken = default);
}

public class InvoiceAttachmentInfo
{
    public int Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid CustomerId { get; set; }
    public string StoredFileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string UploadedUserId { get; set; } = string.Empty;
}
