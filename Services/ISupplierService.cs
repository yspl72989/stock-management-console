namespace StockApi.Services;

public interface ISupplierService
{
    void PlaceOrder(string name, int quantity);
}
