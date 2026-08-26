using StockApi.Models;
using StockApi.Repositories;
using System.Diagnostics;

namespace StockApi.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _repository;

    private static readonly HashSet<string> AllowedUnits =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Kg",
            "Bag",
            "Piece"

        };

    public StockService(IStockRepository repository)
    {
        _repository = repository;
    }

    public List<StockItem> GetAllProducts() => _repository.GetAll();

    public StockItem? GetProductById(int id) => _repository.GetById(id);

    public void AddProduct(StockItem item)
    {
        NormalizeAndValidate(item);

        if (_repository.ExistingByName(item.Name))
        {
            throw new InvalidOperationException(
                $"Prodct '{item.Name}' already exists."
                );
        }
        _repository.Add(item);

    }

    public void UpdateProduct(StockItem item)
    {
        if (item.Id <= 0)
        {
            throw new ArgumentException(
                "Product ID must be greater than zero.");
        }

        var existingProduct = _repository.GetById(item.Id);

        if (existingProduct == null)
        {
            throw new InvalidOperationException("Product does not exist.");
        }

        _repository.Update(item);
    }

    public void DeleteProduct(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Product ID must be greater than zero");
        }

        var product = _repository.GetById(id);

        if (product == null)
        {
            throw new Exception("Product does not exist.");
        }

        _repository.Delete(id);
    }

    private void NormalizeAndValidate(StockItem item)
    {
        item.Name =item.Name.Trim();
        item.Unit = item.Unit.Trim();

        if (string.IsNullOrWhiteSpace(item.Name))
        
        {
            throw new ArgumentException(

               "Product name cannot be empty"
                );
        }
        if (item.Name.Length > 100)
        {
            throw new ArgumentException(
                "Product name cannot be longer than 100 characters. ");
        }

        if (!AllowedUnits.Contains(item.Unit))
        {
            throw new ArgumentException(
                "Unit must be Kg,Bag or Piece");
        }
        item .Unit = NormalizeUnit(item.Unit);

        if (item.Quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        if (decimal.Round(item.Quantity, 3) != item.Quantity)
        {
            throw new ArgumentException(
                "Quantity cannot have more than 3 decimal places.");
        }

        if (item.Unit is "Bag" or "Piece"
            && item.Quantity != decimal.Truncate(item.Quantity))
        {
            throw new ArgumentException(
                $"{item.Unit} quantity must be a whole number.");
        }

        if (item.Price <= 0)
        {
            throw new ArgumentException(
                "Price must be greater than zero.");
        }

        if (decimal.Round(item.Price, 2) != item.Price)
        {
            throw new ArgumentException(
                "Price cannot have more than 2 decimal places.");
        }
    }

    private static string NormalizeUnit(string unit)
    {
        return unit.ToLowerInvariant() switch
        {
            "kg" => "Kg",
            "bag" => "Bag",
            "piece" => "Piece",
            _ => unit

        };
    }
}
