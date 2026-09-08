CREATE OR ALTER PROCEDURE dbo.uspStockItem_Insert
    @Name NVARCHAR(100),
    @Quantity DECIMAL(10,3),
    @Unit NVARCHAR(20),
    @Price DECIMAL(10,2),
    @LastModifiedDate DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.StockItems (Name, Quantity, Unit, Price, LastModifiedDate)
    VALUES (@Name, @Quantity, @Unit, @Price, @LastModifiedDate);
END
GO
