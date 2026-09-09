# Stock Management API

A .NET 8 Web API for managing grocery stock and supplier orders, backed by SQL Server.

## Features

- **Stock management** — CRUD for stock items with validation
- **Stored procedures** — no inline SQL in repositories
- **Supplier service** — place orders for items not in stock and generate invoices
- **Auto database setup** — tables and stored procedures deploy on startup

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

Or press **F5** in Visual Studio with `StockApi.sln` open.

## Database

Connection strings are in `appsettings.json`:

| Setting | Database | Used for |
|---------|----------|----------|
| `StockDb` | `StockManagementDb` | Stock items, orders, stored procedures |
| `MasterDb` | `master` | Startup only — creates `StockManagementDb` if missing |

On first run, `DatabaseInitializer` creates:

- `StockItems` and `StockOrder` tables
- Stored procedures from `Scripts/StoredProcedures/StockItem/` and `Scripts/StoredProcedures/StockOrder/`

No manual SQL setup is required.

Verify in SSMS:

```sql
USE StockManagementDb;

SELECT * FROM dbo.StockItems;
SELECT * FROM dbo.StockOrder;

SELECT name FROM sys.procedures
WHERE name LIKE 'uspStockItem_%' OR name LIKE 'uspStockOrder_%'
ORDER BY name;
```

## Stored procedures

Repositories call stored procedures via `CommandType.StoredProcedure`. Sproc names live in:

| Constants class | Scripts folder |
|-----------------|----------------|
| `Constants/StockItemProcedures.cs` | `Scripts/StoredProcedures/StockItem/` |
| `Constants/StockOrderProcedures.cs` | `Scripts/StoredProcedures/StockOrder/` |

Scripts are copied to the build output and deployed on startup by `DatabaseInitializer`.

## Supplier flow

When a customer orders something the store does not stock (e.g. oil):

1. **Place order** — `SupplierService.PlaceOrder` saves a row to `StockOrder` with `Invoice = N/A`
2. **Generate invoice** — `SupplierService.GenerateInvoice` returns an `InvoiceDto` and sets `Invoice = Send`

If the product exists in stock, unit and price are copied from `StockItems`. Otherwise defaults are used (`Kg`, price `0`).

## API endpoints

### Stock — `/api/stock`

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/stock` | List all stock items |
| GET | `/api/stock/{id}` | Get one item |
| POST | `/api/stock` | Create item |
| PUT | `/api/stock/{id}` | Update item |
| DELETE | `/api/stock/{id}` | Delete item |

**POST body example:**

```json
{
  "name": "Orange",
  "quantity": 5,
  "unit": "Kg",
  "price": 4.50
}
```

Validation rules:

- `unit` must be `Kg`, `Bag`, or `Piece`
- `quantity` must be > 0 (whole number for Bag/Piece; up to 3 decimals for Kg)
- `price` must be > 0 (max 2 decimal places)
- duplicate names return **409 Conflict**

### Orders — `/api/orders`

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/orders` | Place a supplier order |
| POST | `/api/orders/{orderId}/invoice` | Generate invoice for an order |

**Place order body:**

```json
{
  "name": "Oil",
  "quantity": 2
}
```

Expected: **201 Created**

**Generate invoice:** `POST /api/orders/1/invoice` (no body)

Expected: **200 OK** with invoice JSON:

```json
{
  "orderId": 1,
  "name": "Oil",
  "quantity": 2,
  "unit": "Kg",
  "price": 0,
  "total": 0,
  "generatedAt": "2026-09-09T10:00:00Z"
}
```

## Swagger test order

1. `POST /api/stock` — create a product (optional, for in-stock order test)
2. `POST /api/orders` — place an order
3. Find the order id in SSMS: `SELECT Id, Name FROM dbo.StockOrder ORDER BY Id DESC`
4. `POST /api/orders/{id}/invoice` — generate invoice

## Integration tests

Tests live in `StockApi.Test.IntegrationTests/` using **xUnit** and **WebApplicationFactory**.

```bash
dotnet test StockApi.sln
```

| Test class | What it covers |
|------------|----------------|
| `EndpointTests` | Stock API CRUD via HTTP |
| `ServiceTests` | `StockService` recently modified products |
| `StockOrderRepositoryTests` | Order repository create/get |
| `SupplierServiceTests` | Place order, generate invoice, not-found |
| `OrderManagementTests` | Order + invoice endpoints via HTTP |

Requirements: SQL Server running with the same connection strings as the API.

## Solution layout

```
StockApi.csproj                          Web API entry point
Controllers/                             StockController, OrderManagementController
Services/                                StockService, SupplierService
Repositories/                            ADO.NET repositories (stored procedures)
Models/                                  StockItem, StockOrder
Models/Dtos/                              InvoiceDto, PlaceOrderRequest
Constants/                               Sproc names, InvoiceStatus, defaults
Data/                                    DatabaseInitializer, StockDb
Scripts/StoredProcedures/StockItem/      Stock item sprocs
Scripts/StoredProcedures/StockOrder/     Stock order sprocs
StockApi.Test.IntegrationTests/          Integration tests
```

## Architecture

```
Stock:
  HTTP → StockController → StockService → StockRepository → uspStockItem_* → SQL Server

Supplier:
  HTTP → OrderManagementController → SupplierService → StockOrderRepository → uspStockOrder_* → SQL Server
                                                      ↘ StockRepository (stock lookup)
```

## Requirements checklist

| Requirement | How it is met |
|-------------|---------------|
| Avoid hardcoding | `InvoiceStatus`, `StockItemProcedures`, `StockOrderProcedures`, `SupplierOrderDefaults` |
| Avoid inline SQL | All repository data access uses stored procedures |
| Supplier place order | `SupplierService.PlaceOrder`, `POST /api/orders` |
| Generate invoice | `SupplierService.GenerateInvoice`, `POST /api/orders/{id}/invoice`, returns `InvoiceDto` |
