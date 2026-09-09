namespace StockApi.Models.Dtos;

public class PlaceOrderRequest
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
