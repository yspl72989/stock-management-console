CREATE OR ALTER PROCEDURE dbo.uspStockItem_CheckUpdates
    @LastModifiedDate DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate
    FROM dbo.StockItems
    WHERE LastModifiedDate >= @LastModifiedDate
    ORDER BY Id;
END
GO
