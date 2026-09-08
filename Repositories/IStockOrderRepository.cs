using StockApi.Models;

namespace StockApi.Repositories;

public interface IStockOrderRepository
{
    List<StockOrder> GetAll();
    StockOrder? GetByName(string name);
    StockOrder? GetById(int id);
    void Create(StockOrder order);
    void Update(StockOrder order);
    void Delete(int id);
}
