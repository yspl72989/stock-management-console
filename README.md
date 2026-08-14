# StockCli

## Current implementation status

The application demonstrates the complete data flow:

```
Program.cs → StockService → StockRepository → SQL Server
```


1. Creates the database connection
2. Creates repository and service dependencies
3. Calls service methods directly
4. Retrieves and displays products from SQL Server
5. Tests inserting new stock items through the service layer

- Insert, update, delete, and view products from the console
- Duplicate product name checking
- Unit support: `Kg`, `Bag`, `Piece`
- Decimal quantity (e.g. `1.5` kg)
- Validation for name, unit, quantity, and price

## Prerequisites

- .NET 8 SDK
- SQL Server with a `StockManagementDb` database
- `StockItems` table with columns: `Id`, `Name`, `Quantity`, `Unit`, `Price`



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



## Design notes

- Interfaces separate contracts from implementations
- Constructor injection wires repository into service
- SQL uses parameterized queries via ADO.NET
- Business logic stays out of the console and repository layers
