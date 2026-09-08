CREATE OR ALTER PROCEDURE dbo.uspStockOrder_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.StockOrder
    WHERE Id = @Id;
END
GO
