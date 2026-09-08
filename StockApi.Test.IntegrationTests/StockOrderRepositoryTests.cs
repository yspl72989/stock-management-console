using Microsoft.Extensions.DependencyInjection;
using StockApi.Models;
using StockApi.Repositories;
using StockApi.Test.IntegrationTests.Fixtures;

namespace StockApi.Test.IntegrationTests;

[Collection("IntegrationTests")]
public sealed class StockOrderRepositoryTests
{
    private readonly IStockOrderRepository _repository;

    public StockOrderRepositoryTests(WebApplicationFactoryFixture factory)
    {
        _repository = factory.Services.GetRequiredService<IStockOrderRepository>();
    }

    [Fact]
    public void GivenNewOrder_WhenCreatedAndGetByName_ThenOrderIsReturned()
    {
        var order = new StockOrder
        {
            Name = $"Order-{Guid.NewGuid()}",
            Quantity = 2,
            Unit = "Kg",
            Price = 10,
            LastModifiedDate = DateTime.UtcNow,
            Invoice = "N/A"
        };

        _repository.Create(order);

        var result = _repository.GetByName(order.Name);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(order.Name, result.Name);
        Assert.Equal(order.Quantity, result.Quantity);
        Assert.Equal(order.Unit, result.Unit);
        Assert.Equal(order.Price, result.Price);
        Assert.Equal(order.Invoice, result.Invoice);
    }
}
