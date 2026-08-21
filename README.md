# StockCli

A .NET 8 stock management console app with a layered ADO.NET architecture.

## Quick start

You do **not** need to create the database or tables manually. The app runs a `DatabaseInitializer` on startup.

```bash
git clone <your-repo-url>
cd StockCli
dotnet restore
dotnet run
```

On first run, the initializer will:

1. Connect to SQL Server using the `MasterDb` connection string
2. Create the application database if it does not exist
3. Create the `StockItems` table if it does not exist

After that, the interactive CLI menu opens and you can start managing stock.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local instance — e.g. `(localdb)\MSSQLLocalDB` or `Server=.`)

No manual database setup is required.

## Configuration

Connection strings live in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "StockDb": "Server=.;Database=StockManagementDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "MasterDb": "Server=.;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

| Setting | Purpose |
|---------|---------|
| `StockDb` | Application database used by the CLI and repository |
| `MasterDb` | Used only by `DatabaseInitializer` to create the app database |

Update `Server=.` if your SQL Server instance name is different.

## Database initializer

`Data/DatabaseInitializer.cs` runs automatically when the app starts (see `Program.cs`).



The initializer is safe to run repeatedly — it only creates what is missing.

## Current implementation status

The application demonstrates the complete data flow:

```text
Program.cs → DatabaseInitializer → StockService → StockRepository → SQL Server
```

Features:

- Insert, update, delete, and view products from the console
- Duplicate product name checking
- Unit support: `Kg`, `Bag`, `Piece`
- Decimal quantity (e.g. `1.5` kg)
- Validation for name, unit, quantity, and price

## Menu options

1. Insert product
2. Update product
3. Delete product
4. View products
5. Exit

## Validation rules

| Field | Rule |
|-------|------|
| Name | Required, max 100 characters, unique |
| Unit | Must be Kg, Bag, or Piece |
| Quantity | Greater than zero; max 3 decimal places; Bag/Piece must be whole numbers |
| Price | Greater than zero; max 2 decimal places |

## Coming soon

- **Web API controllers** — HTTP endpoints for the same stock operations (planned)

The service and repository layers are already structured so controllers can reuse the same `IStockService` without changing business logic.

## Design notes

- Interfaces separate contracts from implementations
- Constructor injection wires repository into service
- SQL uses parameterized queries via ADO.NET
- Business logic stays out of the console and repository layers
- Database setup is handled at startup — no separate migration step for local development
