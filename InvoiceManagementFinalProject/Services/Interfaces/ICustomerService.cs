using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;

namespace InvoiceManagementFinalProject.Services.Interfaces
{

    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerRequest createCustomerRequest);
        Task<CustomerResponseDto> UpdateCustomerAsync(Guid id, UpdateCustomerRequest createCustomerRequest);
        Task<List<CustomerResponseDto>> GetAllCustomersAsync();
        Task<PagedResult<CustomerResponseDto>> GetPagedAsync(CustomerQueryParams customerQueryParams);
        Task<CustomerResponseDto> GetCustomerByIdAsync(Guid customerId);
        Task<CustomerResponseDto?> ArchiveCustomerAsync(Guid id);
        Task<bool> DeleteCustomerAsync(Guid id);



    }
}