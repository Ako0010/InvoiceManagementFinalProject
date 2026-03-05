using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;

namespace InvoiceManagementFinalProject.Services.Interfaces
{

    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerRequest createCustomerRequest, string currentUserId);
        Task<CustomerResponseDto> UpdateCustomerAsync(Guid id, UpdateCustomerRequest createCustomerRequest, string currentUserId);
        Task<List<CustomerResponseDto>> GetAllCustomersAsync(string currentUserId);
        Task<PagedResult<CustomerResponseDto>> GetPagedAsync(CustomerQueryParams customerQueryParams, string currentUserId);
        Task<CustomerResponseDto> GetCustomerByIdAsync(Guid customerId, string currentUserId);
        Task<CustomerResponseDto?> ArchiveCustomerAsync(Guid id, string currentUserId);
        Task<bool> DeleteCustomerAsync(Guid id, string currentUserId);



    }
}