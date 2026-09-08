CREATE OR ALTER PROCEDURE dbo.uspStockOrder_Insert
    @Name NVARCHAR(100),
    @Quantity DECIMAL(10,3),
    @Unit NVARCHAR(20),
    @Price DECIMAL(10,2),
    @LastModifiedDate DATETIME2(2),
    @Invoice NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.StockOrder (Name, Quantity, Unit, Price, LastModifiedDate, Invoice)
    VALUES (@Name, @Quantity, @Unit, @Price, @LastModifiedDate, @Invoice);
END
GO
