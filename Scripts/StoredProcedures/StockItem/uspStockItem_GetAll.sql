CREATE OR ALTER PROCEDURE dbo.uspStockItem_GetAll
AS
BEGIN
--you just don't get "(1 row(s) affected)" back
    SET NOCOUNT ON;

    SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate
    FROM dbo.StockItems
    ORDER BY Id;
END
GO
