using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;

namespace InvoiceManagementFinalProject.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceResponseDto>> GetAllInvoicesAsync();
        Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(InvoiceQueryParams invoiceQueryParams);
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id);
        Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceRequest createInvoiceRequest);
        Task<InvoiceResponseDto?> UpdateInvoiceAsync(Guid id, UpdateInvoiceRequest updateInvoiceRequest);
        Task<InvoiceResponseDto?> ChangeInvoiceStatusAsync(Guid id, ChangeStatusInvoiceRequest changeStatusInvoiceRequest);
        Task<bool> DeleteInvoiceAsync(Guid id);
        Task<InvoiceResponseDto?> ArchiveInvoiceAsync(Guid id);



    }
}
