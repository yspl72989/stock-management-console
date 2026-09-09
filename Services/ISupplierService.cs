using StockApi.Models.Dtos;

namespace StockApi.Services;

public interface ISupplierService
{
    void PlaceOrder(string name, int quantity);
    InvoiceDto GenerateInvoice(int orderId);
}
