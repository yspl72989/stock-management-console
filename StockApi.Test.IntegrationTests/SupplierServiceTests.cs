using Microsoft.Extensions.DependencyInjection;
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
        Assert.Equal("N/A", order.Invoice);
    }

}
