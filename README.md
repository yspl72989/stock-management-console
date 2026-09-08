# Stock Management API

A .NET 8 Web API for managing stock items, backed by SQL Server. StockItem data access uses stored procedures deployed from `Scripts/StoredProcedures/StockItem/` on startup.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance on `Server=.`)
- Visual Studio 2022 or VS Code (optional)

## Quick start

```bash
dotnet restore StockApi.sln
dotnet run --project StockApi.csproj
```

Open Swagger UI: [https://localhost:51474/swagger](https://localhost:51474/swagger)

Or press **F5** in Visual Studio with `StockApi.sln` open — it launches straight to Swagger.

## Database

Connection strings are in `appsettings.json`:

| Setting | Database | Used for |
|---------|----------|----------|
| `StockDb` | `StockManagementDb` | All stock data (`StockItems` table) |
| `MasterDb` | `master` | Startup only — creates `StockManagementDb` if missing |

On first run, `DatabaseInitializer` creates the database and table automatically. No manual SQL setup is required.

To verify data after testing, connect in SSMS and run:

```sql
USE StockManagementDb;
SELECT * FROM dbo.StockItems;
```

## How to test with Swagger

1. Run the API (see Quick start above).
2. In Swagger, expand the **Stock** section.
3. Try the endpoints in this order:

### POST — create a stock item

`POST /api/stock` → **Try it out** → use this body:

```json
{
  "name": "Orange",
  "quantity": 5,
  "unit": "Kg",
  "price": 4.50
}
```

Expected: **201 Created**

Validation rules:
- `unit` must be `Kg`, `Bag`, or `Piece`
- `quantity` must be > 0 (whole number for Bag/Piece; up to 3 decimals for Kg)
- `price` must be > 0 (max 2 decimal places)
- duplicate names return **409 Conflict**

### GET — list or fetch one item

- `GET /api/stock` — returns all items
- `GET /api/stock/{id}` — returns one item by id (use an id from the list)

### PUT — update an item

`PUT /api/stock/{id}` → body example:

```json
{
  "name": "Orange",
  "quantity": 10,
  "unit": "Kg",
  "price": 5.00
}
```

Expected: **204 No Content**, then confirm with `GET /api/stock/{id}`.

### DELETE — remove an item

`DELETE /api/stock/{id}` → Expected: **204 No Content**

## Integration tests

Automated API tests live in `StockApi.Test.IntegrationTests/` using **xUnit**, **WebApplicationFactory**, and a shared `[Collection("IntegrationTests")]` fixture.

| Test | What it checks |
|------|----------------|
| `GivenNewStock_WhenGetById_ThenIdMatched` | POST → GET all (find id) → GET by id |
| `GivenExistingStock_WhenUpdated_ThenStockIsUpdated` | POST → PUT → GET confirms update |

Run integration tests:

```bash
dotnet test StockApi.Test.IntegrationTests\StockApi.Test.IntegrationTests.csproj
```

Requirements: SQL Server must be running (same connection strings as the API).

## Solution layout

```
StockApi.csproj                        Web API (controllers, services, repositories)
StockApi.Test.IntegrationTests/        Integration tests (WebApplicationFactory + collection fixture)
StockApi.Tests/                        Unit tests (placeholder)
Controllers/                 REST endpoints
Services/                    Business logic and validation
Repositories/                ADO.NET data access
Data/                        DatabaseInitializer, StockDb
```

## Architecture

```
HTTP request → StockController → StockService → StockRepository → SQL Server
```

## Next steps

1. **More integration tests** — DELETE, duplicate name (409), validation errors (400)
2. **Unit tests** — cover `StockService` validation rules in isolation
3. **Improve POST response** — return the created item and id in the 201 response

