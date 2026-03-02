using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;

namespace InvoiceManagementFinalProject.Services.Interfaces
{

    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerRequest createCustomerRequest, string currentUserId);
        Task<CustomerResponseDto> UpdateCustomerAsync(Guid id, UpdateCustomerRequest createCustomerRequest);
        Task<List<CustomerResponseDto>> GetAllCustomersAsync(string currentUserId);
        Task<PagedResult<CustomerResponseDto>> GetPagedAsync(CustomerQueryParams customerQueryParams);
        Task<CustomerResponseDto> GetCustomerByIdAsync(Guid customerId, string currentUserId);
        Task<CustomerResponseDto?> ArchiveCustomerAsync(Guid id);
        Task<bool> DeleteCustomerAsync(Guid id);



    }
}