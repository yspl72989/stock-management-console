using Microsoft.Extensions.DependencyInjection;
using StockApi.Models;
using StockApi.Services;
using StockApi.Test.IntegrationTests.Fixtures;

namespace StockApi.Test.IntegrationTests;

[Collection("IntegrationTests")]
public sealed class ServiceTests
{
    private readonly IStockService _stockService;

    public ServiceTests(WebApplicationFactoryFixture factory)
    {
        _stockService = factory.Services.GetRequiredService<IStockService>();
    }

    [Fact]
    public void GivenRecentStock_WhenUpdatedDateLessThan30Days_ReturnsMatchingItems()
    {

        var item = new StockItem
        {
            Name = $"Recent-{Guid.NewGuid()}",
            Quantity = 1,
            Unit = "Kg",
            Price = 1
        };
        _stockService.AddProduct(item);

        var cutoffDate = DateTime.UtcNow.AddDays(-30);

        var result = _stockService.CheckRecentlyModifiedProducts(cutoffDate);

        Assert.NotNull(result);
        Assert.Contains(result , x=>x.Name == item.Name);
    }

    // gropcery have own stuff, other items from other suppliers, like meat from its own, supplier service 
    // place an order to supplier : supplier needs to generate the invoice (email/html) and send the oreder
    // 2 services talk to each other
    // more than 1 tables.. 
    /*[Fact]
    public void GivenRecentStock_WhenUpdatedDateMoreThan30Days_ReturnEmpty()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-40);

        var result = _stockService.CheckRecentlyModifiedProducts(cutoffDate);

        Assert.Empty(result);
    }*/

}
