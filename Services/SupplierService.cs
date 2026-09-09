using StockApi.Constants;
using StockApi.Models;
using StockApi.Models.Dtos;
using StockApi.Repositories;

namespace StockApi.Services;

public class SupplierService : ISupplierService
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockOrderRepository _orderRepository;

    public SupplierService(
        IStockRepository stockRepository,
        IStockOrderRepository orderRepository)
    {
        _stockRepository = stockRepository;
        _orderRepository = orderRepository;
    }

// order from existing stock items no unit setup
    public void PlaceOrder(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name cannot be empty.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        var productName = name.Trim();

        var product = _stockRepository.GetAll()
        //FirstOrDefault is a LINQ method that returns the first element of a sequence or a default value if no element is found.
            .FirstOrDefault(item => item.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        
//Updated PlaceOrder — lookup in stock when possible, otherwise still save the order.
// if product exists in stock, copy name/unit/price; otherwise accept the order with defaults
        var order = new StockOrder
        {
            Name = product?.Name ?? productName,
            Quantity = quantity,
            Unit = product?.Unit ?? "Kg",//default unit is Kg
            Price = product?.Price ?? 0m,//default price is 0
            LastModifiedDate = DateTime.UtcNow,
            Invoice = "N/A"//default invoice is N/A
        };

        _orderRepository.Create(order);
    }

    public InvoiceDto GenerateInvoice(int orderId)
    {
        if (orderId <= 0)
        {
            throw new ArgumentException("Order ID must be greater than zero.");
        }

        var order = _orderRepository.GetById(orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {orderId} was not found.");
        }

        var generatedAt = DateTime.UtcNow;

        var invoice = new InvoiceDto
        {
            OrderId = order.Id,
            Name = order.Name,
            Quantity = order.Quantity,
            Unit = order.Unit,
            Price = order.Price,
            Total = order.Quantity * order.Price,
            GeneratedAt = generatedAt
        };

        order.Invoice = InvoiceStatus.Send;
        order.LastModifiedDate = generatedAt;
        _orderRepository.Update(order);

        return invoice;
    }
}
