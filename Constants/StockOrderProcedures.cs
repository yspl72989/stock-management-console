namespace StockApi.Constants;

internal static class StockOrderProcedures
{
    public const string GetAll = "dbo.uspStockOrder_GetAll";
    public const string GetById = "dbo.uspStockOrder_GetById";
    public const string GetByName = "dbo.uspStockOrder_GetByName";
    public const string Insert = "dbo.uspStockOrder_Insert";
    public const string Update = "dbo.uspStockOrder_Update";
    public const string Delete = "dbo.uspStockOrder_Delete";
}
