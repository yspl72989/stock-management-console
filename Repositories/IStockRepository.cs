using StockApi.Models;

namespace StockApi.Repositories;

public interface IStockRepository
{
    List<StockItem> GetAll();
    StockItem? GetById(int id);
    bool ExistingByName(string name, int? excludeId = null);
    void Add(StockItem item);
    void Update(StockItem item);
    void Delete(int id);
}
