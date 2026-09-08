using System.Net;
using System.Net.Http.Json;
using StockApi.Models;
using StockApi.Test.IntegrationTests.Fixtures;

namespace StockApi.Test.IntegrationTests;

[Collection("IntegrationTests")]
public sealed class EndpointTests
{
    private readonly HttpClient _httpClient;

    public EndpointTests(WebApplicationFactoryFixture factory)
    {
        _httpClient = factory.HttpClient;
    }

    //server is not working , assert the server is working (log/ timeout)
    // mock the inlinedate .. unit test
    // serive is not working, then integration failed, but wanna unit test passed
    //different mocks mock ui/service
    [Fact]
    public async Task GivenNewStock_WhenGetById_ThenIdMatched()
    {
        var item = new StockItem
        {
            Name = $"ApiGet-{Guid.NewGuid()}",
            Quantity = 1,
            Unit = "Kg",
            Price = 1
            // no LastModifiedDate here
        };

        var postResponse = await _httpClient.PostAsJsonAsync("api/stock", item);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var allItems = await (await _httpClient.GetAsync("api/stock"))
            .Content.ReadFromJsonAsync<List<StockItem>>();

        var createdId = allItems!.First(i => i.Name == item.Name).Id;

        var getByIdResponse = await _httpClient.GetAsync($"api/stock/{createdId}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var result = await getByIdResponse.Content.ReadFromJsonAsync<StockItem>();

        Assert.NotNull(result);
        Assert.Equal(createdId, result.Id);
        Assert.Equal(item.Name, result.Name);
        Assert.Equal(item.Quantity, result.Quantity);
        Assert.Equal("Kg", result.Unit);
        Assert.Equal(item.Price, result.Price);
    }

    [Fact]
    public async Task GivenExistingStock_WhenUpdated_ThenStockIsUpdated()
    {
        var item = new StockItem
        {
            Name = $"ApiPut-{Guid.NewGuid()}",
            Quantity = 1,
            Unit = "Kg",
            Price = 1
        };

        var postResponse = await _httpClient.PostAsJsonAsync("api/stock", item);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var allItems = await (await _httpClient.GetAsync("api/stock"))
            //deserilioze from json to c
            // two lines being together 
            //var response = await _httpClient.GetAsync("api/stock");
            // var allItems = await response.Content.ReadFromJsonAsync<List<StockItem>>();

            .Content.ReadFromJsonAsync<List<StockItem>>();

        //find id because we will use it for check/get all

        var createdId = allItems!.First(i => i.Name == item.Name).Id;

        var updatedItem = new StockItem
        {
            Name = item.Name,
            Quantity = 2,
            Unit = "Kg",
            Price = 5
        };

        var putResponse = await _httpClient.PutAsJsonAsync(
            $"api/stock/{createdId}",
            updatedItem);

        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var getResponse = await _httpClient.GetAsync($"api/stock/{createdId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var result = await getResponse.Content.ReadFromJsonAsync<StockItem>();

        Assert.NotNull(result);
        Assert.Equal(createdId, result.Id);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(5, result.Price);
    }
    
    [Fact]
    public async Task GivenExistingStock_WhenDeleted_ThenItemRemoved()
    {
        var item = new StockItem
        {
            Name = $"ApiDelete-{Guid.NewGuid()}",
            Quantity = 1,
            Unit = "Kg",
            Price = 1
        };
        var postResponse = await _httpClient.PostAsJsonAsync("api/stock", item);

        Assert.NotNull(postResponse);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var allItems = await (await _httpClient.GetAsync("api/stock"))
            .Content.ReadFromJsonAsync<List<StockItem>>();

        var createdId = allItems!.First(i => i.Name == item.Name).Id;

        var deleteResponse = await _httpClient.DeleteAsync($"api/stock/{createdId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _httpClient.GetAsync($"api/stock/{createdId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
