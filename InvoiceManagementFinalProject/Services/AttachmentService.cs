using InvoiceManagementFinalProject.Data;
using InvoiceManagementFinalProject.DTOs;
using InvoiceManagementFinalProject.Models;
using InvoiceManagementFinalProject.Services.Interfaces;
using InvoiceManagementFinalProject.Storage;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagementFinalProject.Services;

public class AttachmentService : IAttachmentService
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;// 5MB
    public static readonly string[] AllowedExtensions = {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf",
        ".txt",
        ".zip"
    };
    public static readonly string[] AllowedContentTypes = {
        "image/jpeg",
        "image/png",
        "application/pdf",
        "text/plain",
        "application/zip",
        "application/x-zip-compressed"
    };

    private readonly HWDbContext _context;
    private readonly IFileStorage _storage;

    public AttachmentService(HWDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<AttachmentResponseDto?> UploadAsync(Guid invoiceId, Stream fileStream, string originalFileName, string contentType, long length, string userId, CancellationToken cancellationToken = default)
    {
        if (length > MaxFileSizeBytes)
            throw new ArgumentOutOfRangeException($"File size must not exceed {MaxFileSizeBytes}");

        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext))
            throw new ArgumentException($"Allowed types: {string.Join(", ", AllowedExtensions)}");

        if (!AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Allowed content types: {string.Join(", ", AllowedExtensions)}");

        var invoice = await _context.Invoices.FindAsync([invoiceId], cancellationToken);

        if (invoice is null)
            return null;

        var folderKey = $"invoices/{invoiceId}";

        var info = await _storage.UploadAsync
            (fileStream, originalFileName, contentType, folderKey, cancellationToken);

        var attachment = new InvoiceAttachment
        {
            InvoiceId = invoiceId,
            OriginalFileName = originalFileName,
            StoredFileName = info.StoredFileName,
            ContentType = contentType,
            Size = info.Size,
            UploadedUserId = userId,
            UploadedAt = DateTimeOffset.UtcNow
        };

        _context.Attachments.Add(attachment);

        await _context.SaveChangesAsync(cancellationToken);

        return new AttachmentResponseDto
        {
            Id = attachment.Id,
            InvoiceId = attachment.InvoiceId,
            OriginalFileName = attachment.OriginalFileName,
            ContentType = attachment.ContentType,
            Size = attachment.Size,
            UploadedUserId = userId,
            UploadedAt = attachment.UploadedAt
        };
    }
    public async Task<(Stream stream, string fileName, string contentType)?> GetDownloadAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId, cancellationToken);

        if (att is null)
            return null;

        var key = $"tasks/{att.InvoiceId}/{att.StoredFileName}";

        var stream = await _storage.OpenReadAsync(key, cancellationToken);

        return (stream, att.OriginalFileName, att.ContentType);
    }
    public async Task<InvoiceAttachmentInfo?> GetAttachmentInfoAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _context.Attachments
                                .Include(i => i.Invoice)
                                .FirstOrDefaultAsync(a => a.Id != attachmentId);

        if (att is null)
            return null;

        return new InvoiceAttachmentInfo
        {
            Id = att.Id,
            InvoiceId = att.InvoiceId,
            CustomerId = att.Invoice.CustomerId,
            StoredFileName = att.StoredFileName,
            StorageKey = $"tasks/{att.InvoiceId}/{att.StoredFileName}",
            UploadedUserId = att.UploadedUserId
        };
    }
    public async Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var att = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == attachmentId);

        if (att is null)
            return false;

        var key = $"tasks/{att.InvoiceId}/{att.StoredFileName}";

        _context.Attachments.Remove(att);

        await _context.SaveChangesAsync(cancellationToken);
        await _storage.DeleteAsync(key, cancellationToken);

        return true;
    }

}
