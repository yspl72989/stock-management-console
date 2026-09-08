using StockApi.Models;
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
}
