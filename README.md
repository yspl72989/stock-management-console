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

**Important:** Set **`StockApi`** as the startup project (not `StockApi.Tests`). To run tests, use **Test Explorer** or `dotnet test` — see [Running tests](#running-tests).

## How it works

### Stock flow

```
Customer / Swagger → POST /api/stock → StockService validates → StockRepository → uspStockItem_Insert → StockItems table
```

Stock items are products the store keeps on the shelf (fruit, veg, etc.).

### Supplier order flow

When a customer wants something the store does not stock (e.g. oil):

```
1. POST /api/orders          → SupplierService.PlaceOrder
                             → saves row to StockOrder (Invoice = "N/A")

2. POST /api/orders/{id}/invoice → SupplierService.GenerateInvoice
                                 → returns InvoiceDto JSON in response body
                                 → updates StockOrder.Invoice to "Send"
```

| Step | What happens in the database | What you see in Swagger |
|------|------------------------------|-------------------------|
| Place order | New row in `StockOrder`; `Invoice = N/A` | **201 Created** (no body) |
| Generate invoice | Same row updated; `Invoice = Send` | **200 OK** with invoice JSON in **response body** |

### Where is the invoice?

The invoice is **not** stored as a separate document or table. It is the **JSON returned** when you call generate invoice.

**In SSMS** you only see a status flag:

```sql
SELECT Id, Name, Quantity, Invoice FROM dbo.StockOrder;
-- Invoice column: "N/A" or "Send"
```

**In Swagger** you see the full invoice in the **Response body** after `POST /api/orders/{orderId}/invoice`:

```json
{
  "orderId": 3,
  "name": "Oil",
  "quantity": 2,
  "unit": "Kg",
  "price": 4.50,
  "total": 9.00,
  "generatedAt": "2026-09-11T05:46:20Z"
}
```

| Field | Meaning |
|-------|---------|
| `orderId` | Order this invoice belongs to |
| `name` | Product name from the order |
| `quantity` | Units ordered |
| `unit` | Kg, Bag, or Piece |
| `price` | Price per unit |
| `total` | `quantity × price` |
| `generatedAt` | When the invoice was generated (UTC) |

There is no `GET` endpoint to fetch an invoice later — call `POST .../invoice` again to regenerate the JSON from the order row.

### Weird names in the database (e.g. `ApiGet-a1b2c3d4-...`)

If you see names like `SupplierOrder-ef8d3515-...` or `Order-a78c76d2-...` in SSMS, that is **leftover integration test data**, not a bug in the API.

Integration tests use unique names so repeated runs do not hit duplicate-name errors:

```csharp
Name = $"ApiGet-{Guid.NewGuid()}"   // EndpointTests
Name = $"SupplierOrder-{Guid.NewGuid()}"   // SupplierServiceTests
```

Tests and Swagger share the same database (`StockManagementDb`) and tests do not clean up after themselves.

When **you** test manually in Swagger with `"name": "Oil"`, the saved name is **`Oil`**. Manual demo flow below uses real names.

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

## Swagger demo (manual test with real names)

Use this flow to see a clean invoice with a normal product name.

### 1. Create stock (optional — copies unit/price onto the order)

`POST /api/stock`

```json
{
  "name": "Oil",
  "quantity": 10,
  "unit": "Kg",
  "price": 4.50
}
```

Expected: **201 Created**

### 2. Place a supplier order

`POST /api/orders`

```json
{
  "name": "Oil",
  "quantity": 2
}
```

Expected: **201 Created**

### 3. Get the order id

Swagger does not return the new order id. In SSMS:

```sql
USE StockManagementDb;
SELECT Id, Name, Invoice FROM dbo.StockOrder ORDER BY Id DESC;
```

Note the `Id` for your `"Oil"` row (e.g. `16`).

### 4. Generate invoice — **this is the invoice**

`POST /api/orders/16/invoice` (no request body)

Expected: **200 OK** — read the **Response body**:

```json
{
  "orderId": 16,
  "name": "Oil",
  "quantity": 2,
  "unit": "Kg",
  "price": 4.50,
  "total": 9.00,
  "generatedAt": "..."
}
```

In SSMS, the same row now has `Invoice = Send`.

## Running tests

| Goal | Command / action |
|------|------------------|
| Run API + Swagger | Set startup project to **`StockApi`**, press F5 |
| Run integration tests | `dotnet test StockApi.Test.IntegrationTests\StockApi.Test.IntegrationTests.csproj` |
| Do **not** F5 on `StockApi.Tests` | Empty placeholder project — will crash with `System.Runtime` error |

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


## Next plan (improvements)

Items to tackle next — especially cleaning up test data and improving the developer experience.

### Test data and database

- [ ] **Use a separate test database** — point `StockApi.Test.IntegrationTests/appsettings.json` at `StockManagementDb_test` so manual Swagger data in `StockManagementDb` stays clean
- [ ] **Stop using GUID names in tests** (or use them only with cleanup) — use fixed names with test teardown, or delete test rows in `Dispose` / `IAsyncLifetime`
- [ ] **Clean up after integration tests** — delete `StockItems` / `StockOrder` rows created in each test (or truncate in fixture teardown)
- [ ] **Document or script manual DB cleanup** for existing test rows (`DELETE ... WHERE Name LIKE 'Api%'`)

### API improvements

- [ ] **Return order id from `POST /api/orders`** — e.g. 201 with `{ "orderId": 16 }` so Swagger demo does not need SSMS
- [ ] **`GET /api/orders/{id}/invoice`** — retrieve invoice JSON without re-posting (optional; would need to store invoice or rebuild from order)
- [ ] **Prevent double invoice** — reject or warn if `Invoice` is already `Send` when generating again

### Tests and project hygiene

- [ ] **Remove or populate `StockApi.Tests`** — add unit tests for `StockService` validation, or remove empty project to avoid F5 confusion
- [ ] **Add unit tests** for validation rules (duplicate name, invalid unit, quantity decimals)
- [ ] **More integration tests** — out-of-stock place order, duplicate invoice edge cases

### Invoice format (stretch)

- [ ] **HTML or PDF invoice** — if required beyond JSON (email template, printable view)
- [ ] **Persist invoice** — optional `Invoice` table if you need history separate from `StockOrder.Invoice` flag
