using Microsoft.Data.SqlClient;

namespace StockCli.Data;

internal class DatabaseInitializer
{
    private readonly string _stockConnectionString;
    private readonly string _masterConnectionString;

    public DatabaseInitializer(
        string stockConnectionString,
        string masterConnectionString)
    {
        _stockConnectionString = stockConnectionString;
        _masterConnectionString = masterConnectionString;
    }

    public void Initialize()
    {
        CreateDatabase();
        CreateStockItemsTable();
    }

    private void CreateDatabase()
    {
        using var connection = new SqlConnection(_masterConnectionString);
        connection.Open();

        const string sql = """
            IF DB_ID('StockManagementDb') IS NULL
            BEGIN
                CREATE DATABASE StockManagementDb;
            END
            """;

        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void CreateStockItemsTable()
    {
        using var connection = new SqlConnection(_stockConnectionString);
        connection.Open();

        const string sql = """
        IF OBJECT_ID('dbo.StockItems', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.StockItems
            (
                Id INT IDENTITY(1,1) NOT NULL,
                Name NVARCHAR(100) NOT NULL,
                Quantity DECIMAL(10,3) NOT NULL,
                Price DECIMAL(10,2) NOT NULL,
                Unit NVARCHAR(20) NOT NULL,

                CONSTRAINT PK_StockItems
                    PRIMARY KEY (Id)
            );
        END
        """;

        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }
}