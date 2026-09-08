CREATE OR ALTER PROCEDURE dbo.uspStockOrder_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate, Invoice
    FROM dbo.StockOrder
    ORDER BY Id;
END
GO
