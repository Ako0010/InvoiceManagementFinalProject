using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

namespace InvoiceManagementFinalProject.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync(string currentUserId);
        Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(InvoiceQueryParams invoiceQueryParams);
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id);
        Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequest createInvoiceRequest, string currentUserId);
        Task<InvoiceResponseDto?> UpdateInvoiceAsync(Guid id, UpdateInvoiceRequest updateInvoiceRequest);
        Task<InvoiceResponseDto?> ChangeInvoiceStatusAsync(Guid id, ChangeStatusInvoiceRequest changeStatusInvoiceRequest);
        Task<bool> DeleteInvoiceAsync(Guid id);
        Task<InvoiceResponseDto?> ArchiveInvoiceAsync(Guid id);
        Task<(byte[] Content, string FileName, string ContentType)?> DownloadInvoiceAsync(Guid id, string format);

    }
}
