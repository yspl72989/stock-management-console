using Microsoft.Extensions.DependencyInjection;
using StockApi.Constants;
using StockApi.Models;
using StockApi.Repositories;
using StockApi.Services;
using StockApi.Test.IntegrationTests.Fixtures;

namespace StockApi.Test.IntegrationTests;

[Collection("IntegrationTests")]
public sealed class SupplierServiceTests
{
    private readonly ISupplierService _supplierService;
    private readonly IStockService _stockService;
    private readonly IStockOrderRepository _orderRepository;

    public SupplierServiceTests(WebApplicationFactoryFixture factory)
    {
        _supplierService = factory.Services.GetRequiredService<ISupplierService>();
        _stockService = factory.Services.GetRequiredService<IStockService>();
        _orderRepository = factory.Services.GetRequiredService<IStockOrderRepository>();
    }

    [Fact]
    public void GivenProductInStock_WhenPlaceOrder_ThenOrderUsesStockDetails()
    {
        var productName = $"SupplierOrder-{Guid.NewGuid()}";

        _stockService.AddProduct(new StockItem
        {
            Name = productName,
            Quantity = 10,
            Unit = "Bag",
            Price = 7.50m
        });

        _supplierService.PlaceOrder(productName, 3);

        var order = _orderRepository.GetByName(productName);

        Assert.NotNull(order);
        Assert.Equal(3, order.Quantity);
        Assert.Equal("Bag", order.Unit);
        Assert.Equal(7.50m, order.Price);
        Assert.Equal(InvoiceStatus.NotApplicable, order.Invoice);
    }

    [Fact]
    public void GivenExistingOrder_WhenGenerateInvoice_ThenReturnsInvoiceAndUpdatesOrder()
    {
        var productName = $"InvoiceOrder-{Guid.NewGuid()}";

        _stockService.AddProduct(new StockItem
        {
            Name = productName,
            Quantity = 10,
            Unit = "Kg",
            Price = 4.00m
        });

        _supplierService.PlaceOrder(productName, 2);

        var order = _orderRepository.GetByName(productName);
        Assert.NotNull(order);

        var invoice = _supplierService.GenerateInvoice(order.Id);

        Assert.Equal(order.Id, invoice.OrderId);
        Assert.Equal(productName, invoice.Name);
        Assert.Equal(2, invoice.Quantity);
        Assert.Equal("Kg", invoice.Unit);
        Assert.Equal(4.00m, invoice.Price);
        Assert.Equal(8.00m, invoice.Total);
        Assert.True(invoice.GeneratedAt <= DateTime.UtcNow);

        var updatedOrder = _orderRepository.GetById(order.Id);
        Assert.NotNull(updatedOrder);
        Assert.Equal(InvoiceStatus.Send, updatedOrder.Invoice);
    }

    [Fact]
    public void GivenMissingOrder_WhenGenerateInvoice_ThenThrows()
    {
        Assert.Throws<InvalidOperationException>(() => _supplierService.GenerateInvoice(999999));
    }
}
