CREATE OR ALTER PROCEDURE dbo.uspStockOrder_GetByName
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate, Invoice
    FROM dbo.StockOrder
    WHERE Name = @Name;
END
GO
