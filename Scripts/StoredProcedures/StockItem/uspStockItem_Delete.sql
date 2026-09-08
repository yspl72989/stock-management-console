CREATE OR ALTER PROCEDURE dbo.uspStockItem_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.StockItems
    WHERE Id = @Id;
END
GO
