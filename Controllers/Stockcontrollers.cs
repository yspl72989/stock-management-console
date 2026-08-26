using Microsoft.AspNetCore.Mvc;
using StockApi.Models;
using StockApi.Services;

namespace StockApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _service;

    public StockController(IStockService service)
    {
        _service = service;
    }

    // GET: /api/stock
    [HttpGet]
    public ActionResult<List<StockItem>> GetAll()
    {
        var items = _service.GetAllProducts();
        return Ok(items);
    }

    // GET: /api/stock/5
    [HttpGet("{id:int}")]
    public ActionResult<StockItem> GetById(int id)
    {
        var item = _service.GetProductById(id);

        if (item == null)
        {
            return NotFound(new
            {
                message = $"Stock item with ID {id} was not found."
            });
        }

        return Ok(item);
    }

    // POST: /api/stock
    [HttpPost]
    public ActionResult Add([FromBody] StockItem item)
    {
        try
        {
            _service.AddProduct(item);

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: /api/stock/5
    [HttpPut("{id:int}")]
    public ActionResult Update(int id, [FromBody] StockItem item)
    {
        try
        {
            item.Id = id;

            _service.UpdateProduct(item);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // DELETE: /api/stock/5
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        try
        {
            _service.DeleteProduct(id);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}
