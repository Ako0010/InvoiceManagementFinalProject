using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;
using InvoiceManagementFinalProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagementFinalProject.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "User")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;


    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create([FromBody] CreateCustomerRequest createCustomerRequest)
    {
        var createdCustomer = await _customerService.CreateCustomerAsync(createCustomerRequest);
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdCustomer.Id }, 
            ApiResponse<CustomerResponseDto>.SuccessResponse(createdCustomer, "Customer created successfully"));
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(ApiResponse<IEnumerable<CustomerResponseDto>>.SuccessResponse(customers, "Customers returned successfully"));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerResponseDto>>> GetPaged([FromQuery] CustomerQueryParams customerQueryParams)
    {
        var customers = await _customerService.GetPagedAsync(customerQueryParams);
        return Ok(customers);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById([FromBody]Guid id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(customer, "Customer returned successfully"));
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest updateCustomerRequest)
    {
        var updatedCustomer = await _customerService.UpdateCustomerAsync(id, updateCustomerRequest);
        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(updatedCustomer, "Customer updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _customerService.DeleteCustomerAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult> Archive(Guid id)
    {
        var IsArchived = await _customerService.ArchiveCustomerAsync(id);

        if (IsArchived is null)
            return NotFound($"Customer with id {id} Not found");


        return Ok(IsArchived);
    }


}
