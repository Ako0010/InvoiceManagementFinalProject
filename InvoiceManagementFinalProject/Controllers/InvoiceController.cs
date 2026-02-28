using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.Customer_DTOs;
using InvoiceManagementFinalProject.DTOs.Invoice_DTOs;
using InvoiceManagementFinalProject.Services;
using InvoiceManagementFinalProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagementFinalProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{
	private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet("all")]

    public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetAll()
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        return Ok(ApiResponse<IEnumerable<InvoiceResponseDto>>.SuccessResponse(invoices, "Invoices retrieved successfully!"));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<InvoiceResponseDto>>> GetPaged([FromQuery] InvoiceQueryParams invoiceQueryParams)
    {
        var invoices = await _invoiceService.GetPagedAsync(invoiceQueryParams);
        return Ok(ApiResponse<PagedResult<InvoiceResponseDto>>.SuccessResponse(invoices!, "Invoices retrieved successfully!"));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetById(Guid id)
        {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }
        return Ok(ApiResponse<InvoiceResponseDto>.SuccessResponse(invoice, "Invoice retrieved successfully!"));
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> Create([FromBody] CreateInvoiceRequest createInvoiceRequest)
    {
        var createdInvoice = await _invoiceService.CreateInvoiceAsync(createInvoiceRequest);
        if (createdInvoice == null)
            return NotFound($"Customer with ID {createInvoiceRequest.CustomerId} not found");
        return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, createdInvoice);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvoiceResponseDto>> Update(Guid id, [FromBody] UpdateInvoiceRequest request)
    {
        var invoice = await _invoiceService.UpdateInvoiceAsync(id, request);
        if (invoice == null)
            return BadRequest("Invoice not found or cannot be updated (must be in Created status).");

        return Ok(invoice);
    }

    [HttpPost("{id:guid}/status")]
    public async Task<ActionResult<InvoiceResponseDto>> ChangeStatus(Guid id, [FromBody] ChangeStatusInvoiceRequest request)
    {
        var invoice = await _invoiceService.ChangeInvoiceStatusAsync(id, request);
        if (invoice == null)
            return BadRequest("Invoice not found or cannot change status.");
        return Ok(invoice);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _invoiceService.DeleteInvoiceAsync(id);
        if (!success)
            return BadRequest("Invoice not found or cannot be deleted (must be in Created status).");

        return NoContent();
    }

    [HttpPost("{id}/archive")]
    public async Task<ActionResult<InvoiceResponseDto>> Archive(Guid id)
    {
        var isArchive = await _invoiceService.ArchiveInvoiceAsync(id);

        if (isArchive is null)
            return BadRequest($"Customer with id {id} not found");

        return Ok(isArchive);
    }

}
