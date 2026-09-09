using Microsoft.Data.SqlClient;

namespace StockApi.Data;

internal class DatabaseInitializer
{
    private readonly string _stockConnectionString;
    private readonly string _masterConnectionString;
    private readonly string _databaseName;

    public DatabaseInitializer(
        string stockConnectionString,
        string masterConnectionString)
    {
        _stockConnectionString = stockConnectionString;
        _masterConnectionString = masterConnectionString;
        _databaseName = GetDatabaseName(stockConnectionString);
    }

    public void Initialize()
    {
        CreateDatabase();
        CreateStockItemsTable();
        CreateStockOrderTable();
        EnsureLastModifiedDateColumn();
        EnsureStockItemStoredProcedures();
        EnsureStockOrderStoredProcedures();
    }

    private static string GetDatabaseName(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                "StockDb connection string must include a Database name.");
        }

        return builder.InitialCatalog;
    }

    private void CreateDatabase()
    {
        using var connection = new SqlConnection(_masterConnectionString);
        connection.Open();

        var sql = $"""
            IF DB_ID(N'{_databaseName.Replace("'", "''")}') IS NULL
            BEGIN
                CREATE DATABASE [{_databaseName.Replace("]", "]]")}];
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
                LastModifiedDate DATETIME2(2) NOT NULL,

                CONSTRAINT PK_StockItems
                    PRIMARY KEY (Id)
            );
        END
        """;

        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void CreateStockOrderTable()
    {
        using var connection = new SqlConnection(_stockConnectionString);
        connection.Open();

        const string sql = """
        IF OBJECT_ID('dbo.StockOrder', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.StockOrder
            (
                Id INT IDENTITY(1,1) NOT NULL,
                Name NVARCHAR(100) NOT NULL,
                Quantity DECIMAL(10,3) NOT NULL,
                Price DECIMAL(10,2) NOT NULL,
                Unit NVARCHAR(20) NOT NULL,
                LastModifiedDate DATETIME2(2) NOT NULL,
                Invoice NVARCHAR(10) NOT NULL
                    CONSTRAINT DF_StockOrder_Invoice DEFAULT 'N/A',

                CONSTRAINT PK_StockOrder
                    PRIMARY KEY (Id),
                CONSTRAINT CK_StockOrder_Invoice
                    CHECK (Invoice IN ('Send', 'N/A'))
            );
        END
        """;

        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void EnsureLastModifiedDateColumn()
    {
        using var connection = new SqlConnection(_stockConnectionString);
        connection.Open();

        const string sql = """
        IF OBJECT_ID('dbo.StockItems', 'U') IS NOT NULL
           AND COL_LENGTH('dbo.StockItems', 'LastModifiedDate') IS NULL
        BEGIN
            ALTER TABLE dbo.StockItems
            ADD LastModifiedDate DATETIME2(2) NOT NULL
                CONSTRAINT DF_StockItems_LastModifiedDate DEFAULT SYSUTCDATETIME();
        END
        """;

        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void EnsureStockItemStoredProcedures()
    {
        EnsureStoredProcedures("StockItem");
    }

    private void EnsureStockOrderStoredProcedures()
    {
        EnsureStoredProcedures("StockOrder");
    }

    private void EnsureStoredProcedures(string subfolder)
    {
        using var connection = new SqlConnection(_stockConnectionString);
        connection.Open();

        var scriptsDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "Scripts",
            "StoredProcedures",
            subfolder);

        foreach (var scriptPath in Directory.GetFiles(scriptsDirectory, "*.sql"))
        {
            DeployStoredProcedureScript(connection, File.ReadAllText(scriptPath));
        }
    }

    private static void DeployStoredProcedureScript(SqlConnection connection, string sql)
    {
        sql = string.Join(
            '\n',
            sql.Split('\n').Where(line => !line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))).Trim();

        var procedureName = sql.Split('\n')[0]
            .Replace("CREATE OR ALTER PROCEDURE ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        using (var dropCommand = new SqlCommand(
            $"IF OBJECT_ID(N'{procedureName}', N'P') IS NOT NULL DROP PROCEDURE {procedureName};",
            connection))
        {
            dropCommand.ExecuteNonQuery();
        }

        sql = sql.Replace(
            "CREATE OR ALTER PROCEDURE",
            "CREATE PROCEDURE",
            StringComparison.OrdinalIgnoreCase);

        using var createCommand = new SqlCommand(sql, connection);
        createCommand.ExecuteNonQuery();
    }
}
