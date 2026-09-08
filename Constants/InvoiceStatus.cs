namespace StockApi.Constants;

public static class InvoiceStatus
{
    public const string Send = "Send";
    //saved as "N/A" in DB (order recorded, no invoice)
    public const string NotApplicable = "N/A";
}
// A StockOrder has an Invoice column that tracks whether an invoice should be sent to a supplier.
// The database only allows two values: "Send" or "NotApplicable".
/*Invoice = "N/A"     //in a test
Invoice = "Send"     // in SupplierService (future)
= "N/A"              // default on StockOrder
CHECK (Invoice IN ('Send', 'N/A'))  // in SQL*/
//so we create a constant class to store the two values
//This is not a stored procedure constant — it’s a business rule constant (valid data values).