# Stock Management Console

Stock management console app using a layered architecture and SQL Server (ADO.NET).

## Current implementation status

The application demonstrates the complete data flow:

```
Program.cs → StockService → StockRepository → SQL Server
```

The current `Program.cs`:

1. Creates the database connection
2. Creates repository and service dependencies
3. Calls service methods directly
4. Inserts a new stock item through the service layer
5. Retrieves and displays products from SQL Server

Example:

```csharp
Service.AddProduct(new StockItem
{
    Name = "Orange",
    Quantity = 5,
    Price = 4.50m
});

var products = Service.GetAllProducts();
```

## Setup

```bash
dotnet restore
dotnet build
dotnet run
```

Database: `StockManagementDb` on local SQL Server (`.`).

```sql
CREATE TABLE StockItems (
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Name     NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Price    DECIMAL(18,2) NOT NULL
);
```

Update the connection string in `Program.cs` or `appsettings.json` if your SQL Server instance differs.

## Areas for future improvement

1. **Console user interface** — allow users to manage stock without modifying code
2. **Duplicate checking rules** — prevent inserting duplicate products
3. **Better ID handling** — deleted record IDs are not automatically reused (SQL Server `IDENTITY` behaviour)

## Improvements from last code feedback

1. Applied a layered architecture
2. Used interfaces to separate contracts from implementations
3. Improved understanding of dependency injection
4. Reduced coupling between business logic and data access
5. Structured the solution with clear separation of concerns (service and repository layers)
6. Implemented SQL Server data access using ADO.NET instead of placing SQL directly in the console application
