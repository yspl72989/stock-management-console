CREATE OR ALTER PROCEDURE dbo.uspStockItem_ExistingByName
    @Name NVARCHAR(100),
    --check all rows :null
    --excludeId = 5 → check all rows except Id 5 → use on Update for item 5
    @ExcludeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    ----That returns one row, one column — an integer like 0 or 1
    SELECT COUNT(1)
    FROM dbo.StockItems
    WHERE Name = @Name
      AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
END
GO
