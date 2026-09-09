using Microsoft.AspNetCore.Mvc;
using StockApi.Models.Dtos;
using StockApi.Services;

namespace StockApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderManagementController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public OrderManagementController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpPost]
    public ActionResult PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        try
        {
            _supplierService.PlaceOrder(request.Name, request.Quantity);

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{orderId:int}/invoice")]
    public ActionResult<InvoiceDto> GenerateInvoice(int orderId)
    {
        try
        {
            var invoice = _supplierService.GenerateInvoice(orderId);

            return Ok(invoice);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
