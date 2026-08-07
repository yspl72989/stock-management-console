using StockCli.Models;

namespace StockCli.Repositories;

public interface IStockRepository
{
    List<StockItem> GetAll();
    StockItem? GetById(int id);
    void Add(StockItem item);
    void Update(StockItem item);
    void Delete(int id);
}
