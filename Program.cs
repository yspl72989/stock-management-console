using Microsoft.Extensions.Configuration;
using StockCli.Data;
using StockCli.Helper;
using StockCli.Models;
using StockCli.Repositories;
using StockCli.Services;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

string stockConnectionString =
    configuration.GetConnectionString("StockDb")
    ?? throw new InvalidOperationException(
        "StockDb connection string is missing.");

string masterConnectionString =
    configuration.GetConnectionString("MasterDb")
    ?? throw new InvalidOperationException(
        "MasterDb connection string is missing.");

// Database initialization
var initializer = new DatabaseInitializer(
    stockConnectionString,
    masterConnectionString);

initializer.Initialize();

// Database
var db = new StockDb(stockConnectionString);

// Repository
IStockRepository repository = new StockRepository(db);

// Service
IStockService service = new StockService(repository);


while (true)
{
    Console.Clear();

    ShowMainMenu();

    var choice = ConsoleInput.ReadRequiredString("Choose an option: ");

    Console.Clear();

    try
    {
        switch (choice)
        {
            case "1":
                InsertProduct(service);
                break;

            case "2":
                UpdateProduct(service);
                break;

            case "3":
                DeleteProduct(service);
                break;

            case "4":
                ViewProducts(service);
                break;

            case "5":
                Console.WriteLine("Goodbye!");
                return;

            default:
                Console.WriteLine(
                    "Invalid option. Please choose 1-5.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"Error: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine("Press Enter to return to the main menu.");
    Console.ReadLine();
}

static void ShowMainMenu()
{
    Console.WriteLine("=================================");
    Console.WriteLine("       STOCK MANAGEMENT CLI");
    Console.WriteLine("=================================");
    Console.WriteLine();
    Console.WriteLine("1. Insert product");
    Console.WriteLine("2. Update product");
    Console.WriteLine("3. Delete product");
    Console.WriteLine("4. View products");
    Console.WriteLine("5. Exit");
    Console.WriteLine();
}

static void InsertProduct(IStockService service)
{
    Console.WriteLine("=================================");
    Console.WriteLine("          INSERT PRODUCT");
    Console.WriteLine("=================================");
    Console.WriteLine();

    var name = ConsoleInput.ReadRequiredString(
        "Product name: ");

    var unit = ConsoleInput.ReadUnit();

    var quantity = ConsoleInput.ReadQuantity(unit);

    var price = ConsoleInput.ReadPositiveDecimal(
        "Price: ");

    var item = new StockItem
    {
        Name = name,
        Quantity = quantity,
        Unit = unit,
        Price = price
    };

    service.AddProduct(item);

    Console.WriteLine();
    Console.WriteLine("Product added successfully.");
}

static void UpdateProduct(IStockService service)
{
    Console.WriteLine("=================================");
    Console.WriteLine("          UPDATE PRODUCT");
    Console.WriteLine("=================================");
    Console.WriteLine();

    var products = service.GetAllProducts();

    if (products.Count == 0)
    {
        Console.WriteLine("No products found.");
        return;
    }

    DisplayProducts(products);

    Console.WriteLine();

    var id = ConsoleInput.ReadPositiveInt(
        "Enter product ID: ");

    var existing = service.GetProductById(id);

    if (existing == null)
    {
        Console.WriteLine(
            $"Product with ID {id} does not exist.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Current product:");
    DisplayProduct(existing);

    Console.WriteLine();
    Console.WriteLine("What would you like to update?");
    Console.WriteLine("1. Name");
    Console.WriteLine("2. Quantity");
    Console.WriteLine("3. Unit");
    Console.WriteLine("4. Price");
    Console.WriteLine("5. All");
    Console.WriteLine("6. Cancel");

    var choice = ConsoleInput.ReadRequiredString(
        "Choose an option: ");

    switch (choice)
    {
        case "1":
            existing.Name = ConsoleInput.ReadRequiredString(
                "New name: ");
            break;

        case "2":
            existing.Quantity = ConsoleInput.ReadQuantity(
                existing.Unit);
            break;

        case "3":
            existing.Unit = ConsoleInput.ReadUnit();

            existing.Quantity = ValidateQuantityForNewUnit(
                existing.Quantity,
                existing.Unit);

            break;

        case "4":
            existing.Price = ConsoleInput.ReadPositiveDecimal(
                "New price: ");
            break;

        case "5":
            existing.Name = ConsoleInput.ReadRequiredString(
                "New name: ");

            existing.Unit = ConsoleInput.ReadUnit();

            existing.Quantity = ConsoleInput.ReadQuantity(
                existing.Unit);

            existing.Price = ConsoleInput.ReadPositiveDecimal(
                "New price: ");

            break;

        case "6":
            Console.WriteLine("Update cancelled.");
            return;

        default:
            Console.WriteLine(
                "Invalid option.");
            return;
    }

    service.UpdateProduct(existing);

    Console.WriteLine();
    Console.WriteLine("Product updated successfully.");
}

static decimal ValidateQuantityForNewUnit(
    decimal currentQuantity,
    string newUnit)
{
    if (newUnit is "Bag" or "Piece"
        && currentQuantity != decimal.Truncate(currentQuantity))
    {
        Console.WriteLine();
        Console.WriteLine(
            $"The current quantity {currentQuantity} is not valid for {newUnit}.");

        return ConsoleInput.ReadQuantity(newUnit);
    }

    return currentQuantity;
}

static void DeleteProduct(IStockService service)
{
    Console.WriteLine("=================================");
    Console.WriteLine("          DELETE PRODUCT");
    Console.WriteLine("=================================");
    Console.WriteLine();

    var products = service.GetAllProducts();

    if (products.Count == 0)
    {
        Console.WriteLine("No products found.");
        return;
    }

    DisplayProducts(products);

    Console.WriteLine();

    var id = ConsoleInput.ReadPositiveInt(
        "Enter product ID: ");

    var product = service.GetProductById(id);

    if (product == null)
    {
        Console.WriteLine(
            $"Product with ID {id} does not exist.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Product to delete:");
    DisplayProduct(product);

    Console.WriteLine();

    var confirmed = ConsoleInput.ReadYesNo(
        "Are you sure you want to delete this product?");

    if (!confirmed)
    {
        Console.WriteLine("Delete cancelled.");
        return;
    }

    service.DeleteProduct(id);

    Console.WriteLine();
    Console.WriteLine("Product deleted successfully.");
}

static void ViewProducts(IStockService service)
{
    Console.WriteLine("=================================");
    Console.WriteLine("          ALL PRODUCTS");
    Console.WriteLine("=================================");
    Console.WriteLine();

    var products = service.GetAllProducts();

    if (products.Count == 0)
    {
        Console.WriteLine("No products found.");
        return;
    }

    DisplayProducts(products);
}

static void DisplayProducts(List<StockItem> products)
{
    Console.WriteLine(
        "-------------------------------------------------------------");

    Console.WriteLine(
        $"{"ID",-5}{"Name",-20}{"Quantity",-12}{"Price",-10}");

    Console.WriteLine(
        "-------------------------------------------------------------");

    foreach (var product in products)
    {
        var quantityDisplay =
            $"{product.Quantity:0.###} {GetUnitDisplay(product.Unit)}";

        Console.WriteLine(
            $"{product.Id,-5}" +
            $"{product.Name,-20}" +
            $"{quantityDisplay,-12}" +
            $"${product.Price,8:0.00}");
    }

    Console.WriteLine(
        "-------------------------------------------------------------");
}

static void DisplayProduct(StockItem product)
{
    Console.WriteLine($"ID:       {product.Id}");
    Console.WriteLine($"Name:     {product.Name}");
    Console.WriteLine(
        $"Quantity: {product.Quantity:0.###} {GetUnitDisplay(product.Unit)}");
    Console.WriteLine($"Price:    ${product.Price:0.00}");
}

static string GetUnitDisplay(string unit)
{
    return unit switch
    {
        "Piece" => "pieces",
        "Bag" => "bags",
        "Kg" => "kg",
        _ => unit
    };
}