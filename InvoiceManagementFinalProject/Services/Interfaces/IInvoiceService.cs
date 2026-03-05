using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

namespace InvoiceManagementFinalProject.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync(string currentUserId);
        Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(InvoiceQueryParams invoiceQueryParams,string currentUserId);
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id,string currentUserId);
        Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequest createInvoiceRequest, string currentUserId);
        Task<InvoiceResponseDto?> UpdateInvoiceAsync(Guid id, UpdateInvoiceRequest updateInvoiceRequest, string currentUserId);
        Task<InvoiceResponseDto?> ChangeInvoiceStatusAsync(Guid id, ChangeStatusInvoiceRequest changeStatusInvoiceRequest, string currentUserId);
        Task<bool> DeleteInvoiceAsync(Guid id, string currentUserId);
        Task<InvoiceResponseDto?> ArchiveInvoiceAsync(Guid id, string currentUserId);
        Task<(byte[] Content, string FileName, string ContentType)?> DownloadInvoiceAsync(Guid id, string format,string currentUserId);

    }
}
