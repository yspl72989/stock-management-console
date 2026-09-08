CREATE OR ALTER PROCEDURE dbo.uspStockOrder_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate, Invoice
    FROM dbo.StockOrder
    WHERE Id = @Id;
END
GO
