using StockApi.Models;

namespace StockApi.Services;

public interface IStockService
{
    List<StockItem> GetAllProducts();
    StockItem? GetProductById(int id);
    void AddProduct(StockItem item);
    void UpdateProduct(StockItem item);
    void DeleteProduct(int id);
}
