using Microsoft.Extensions.Configuration;
using StockCli.Data;
using StockCli.Models;
using StockCli.Repositories;
using StockCli.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("StockDb")
    ?? throw new InvalidOperationException("Connection string 'StockDb' not found.");

var db = new StockDb(connectionString);
var repository = new StockRepository(db);
var service = new StockService(repository);

var products = service.GetAllProducts();

foreach (var product in products)
{
    Console.WriteLine($"{product.Id} | {product.Name} | {product.Quantity} | {product.Price:F2}");
}
