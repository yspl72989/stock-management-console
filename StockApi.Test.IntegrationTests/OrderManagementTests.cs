using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using StockApi.Models;
using StockApi.Models.Dtos;
using StockApi.Repositories;
using StockApi.Test.IntegrationTests.Fixtures;

namespace StockApi.Test.IntegrationTests;

[Collection("IntegrationTests")]
public sealed class OrderManagementTests
{
    private readonly HttpClient _client;
    private readonly IStockOrderRepository _orderRepository;

    public OrderManagementTests(WebApplicationFactoryFixture factory)
    {
        _client = factory.HttpClient;
        _orderRepository = factory.Services.GetRequiredService<IStockOrderRepository>();
    }

    [Fact]
    public async Task GivenNewOrder_WhenGenerateInvoice_ThenReturnsInvoiceJson()
    {
        //name is a unique identifier for the product
        var productName = $"ApiInvoice-{Guid.NewGuid()}";
        //create a new product in the stock
        var createStockResponse = await _client.PostAsJsonAsync("/api/stock", new StockItem
        {
            Name = productName,
            Quantity = 10,
            Unit = "Piece",
            Price = 3.50m
        });
        Assert.Equal(HttpStatusCode.Created, createStockResponse.StatusCode);

        var placeOrderResponse = await _client.PostAsJsonAsync("/api/orders", new PlaceOrderRequest
        {
            Name = productName,
            Quantity = 4
        });
        Assert.Equal(HttpStatusCode.Created, placeOrderResponse.StatusCode);

        var order = _orderRepository.GetByName(productName);
        Assert.NotNull(order);

        var invoiceResponse = await _client.PostAsync($"/api/orders/{order.Id}/invoice", null);
        Assert.Equal(HttpStatusCode.OK, invoiceResponse.StatusCode);

        var invoice = await invoiceResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        Assert.NotNull(invoice);
        Assert.Equal(order.Id, invoice.OrderId);
        Assert.Equal(productName, invoice.Name);
        Assert.Equal(4, invoice.Quantity);
        Assert.Equal("Piece", invoice.Unit);
        Assert.Equal(3.50m, invoice.Price);
        Assert.Equal(14.00m, invoice.Total);
    }
}
