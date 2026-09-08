namespace StockApi.Constants;
// create this class for easy management 
// without it won't break anything but it's good to have it
// with it, we could easily change the procedure name if needed
// like fewer typeos "dbo.uspStockItem_Delte" won’t compile if you use a constant
internal static class StockItemProcedures
{
    public const string GetAll = "dbo.uspStockItem_GetAll";
    public const string GetById = "dbo.uspStockItem_GetById";
    public const string CheckUpdates = "dbo.uspStockItem_CheckUpdates";
    public const string ExistingByName = "dbo.uspStockItem_ExistingByName";
    public const string Insert = "dbo.uspStockItem_Insert";
    public const string Update = "dbo.uspStockItem_Update";
    public const string Delete = "dbo.uspStockItem_Delete";
}
