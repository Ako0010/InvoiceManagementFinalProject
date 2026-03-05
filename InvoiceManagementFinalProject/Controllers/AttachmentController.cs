using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs;
using InvoiceManagementFinalProject.Services;
using InvoiceManagementFinalProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvoiceManagementFinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly IAuthorizationService _authorizationService;

        private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public AttachmentsController(
            IAttachmentService attachmentService,
            IInvoiceService invoiceService,
            ICustomerService customerService,
            IAuthorizationService authorizationService)
        {
            _attachmentService = attachmentService;
            _invoiceService = invoiceService;
            _customerService = customerService;
            _authorizationService = authorizationService;
        }

        [HttpPost("~/api/invoices/{invoiceId}/attachments")]
        public async Task<ActionResult<ApiResponse<AttachmentResponseDto>>> Upload(
            Guid invoiceId,
            IFormFile file,
            CancellationToken cancellationToken
            )
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId,UserId);
            if (invoice is null)
                return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(invoice.CustomerId,UserId);
            if (customer is null)
                return NotFound();

            if (file is null || file.Length == 0)
                return BadRequest("File is required");

            AttachmentResponseDto? attachment;
            await using var stream = file.OpenReadStream();
            attachment = await _attachmentService.UploadAsync(
                invoiceId,
                stream,
                file.FileName,
                file.ContentType,
                file.Length,
                UserId!,
                cancellationToken
                );

            if (attachment is null)
                return NotFound();

            return Ok(ApiResponse<AttachmentResponseDto>.SuccessResponse(attachment, "File uploaded"));
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
        {
            var info = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);

            if (info is null)
                return NotFound();

            var invoice = await _invoiceService.GetInvoiceByIdAsync(info.InvoiceId,UserId);

            if (invoice is null)
                return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(invoice.CustomerId,UserId);

            if (customer is null)
                return NotFound();


            var result = await _attachmentService.GetDownloadAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return File(result.Value.stream, result.Value.contentType, result.Value.fileName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var info = await _attachmentService.GetAttachmentInfoAsync(id, cancellationToken);

            if (info is null)
                return NotFound();

            var invoice = await _invoiceService.GetInvoiceByIdAsync(info.InvoiceId,UserId);

            var deleted = await _attachmentService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}