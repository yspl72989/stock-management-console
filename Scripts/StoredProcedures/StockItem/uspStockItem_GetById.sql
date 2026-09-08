CREATE OR ALTER PROCEDURE dbo.uspStockItem_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate
    FROM dbo.StockItems
    WHERE Id = @Id;
END
GO
