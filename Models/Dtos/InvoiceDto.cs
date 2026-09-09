namespace StockApi.Models.Dtos;

public class InvoiceDto
{
    public int OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public DateTime GeneratedAt { get; set; }
}
