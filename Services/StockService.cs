using StockCli.Models;
using StockCli.Repositories;

namespace StockCli.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _repository;

    public StockService(IStockRepository repository)
    {
        _repository = repository;
    }

    public List<StockItem> GetAllProducts() => _repository.GetAll();

    public StockItem? GetProductById(int id) => _repository.GetById(id);

    public void AddProduct(StockItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            throw new Exception("Product name cannot be empty.");
        }

        if (item.Quantity < 0)
        {
            throw new Exception("Quantity cannot be negative.");
        }

        _repository.Add(item);
    }

    public void UpdateProduct(StockItem item)
    {
        var existingProduct = _repository.GetById(item.Id);

        if (existingProduct == null)
        {
            throw new Exception("Product does not exist.");
        }

        _repository.Update(item);
    }

    public void DeleteProduct(int id)
    {
        var product = _repository.GetById(id);

        if (product == null)
        {
            throw new Exception("Product does not exist.");
        }

        _repository.Delete(id);
    }
}
