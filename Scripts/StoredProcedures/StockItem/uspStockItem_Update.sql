CREATE OR ALTER PROCEDURE dbo.uspStockItem_Update
    @Id INT,
    @Name NVARCHAR(100),
    @Quantity DECIMAL(10,3),
    @Unit NVARCHAR(20),
    @Price DECIMAL(10,2),
    @LastModifiedDate DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.StockItems
    SET
        Name = @Name,
        Quantity = @Quantity,
        Unit = @Unit,
        Price = @Price,
        LastModifiedDate = @LastModifiedDate
    WHERE Id = @Id;
END
GO
