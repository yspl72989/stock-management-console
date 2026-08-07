using StockCli.Data;
using StockCli.Models;
using StockCli.Repositories;
using StockCli.Services;

string connectionString =
    "Server=.;Database=StockManagementDb;Trusted_Connection=True;TrustServerCertificate=True;";

//database
var db = new StockDb(connectionString);

//Repos
var Repository = new StockRepository(db);

//service
var Service = new StockService(Repository);

//test
Service.AddProduct(new StockItem
{
    Name = "Orange",
    Quantity = 5,
    Price = 4.50m
});
var products = Service.GetAllProducts();

foreach (var product in products)
{
    Console.WriteLine(
        $"{product.Id}{product.Name}{product.Quantity}");
}
