using Microsoft.Data.SqlClient;

namespace StockApi.Data;

public class StockDb
{
    private readonly string _connectionString;

    public StockDb(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection CreateConnection() => new(_connectionString);
}
