using Microsoft.Data.SqlClient;
using StockApi.Data;
using StockApi.Models;

namespace StockApi.Repositories;

public class StockRepository : IStockRepository
{
    private readonly StockDb _db;

    public StockRepository(StockDb db)
    {
        _db = db;
    }

    public List<StockItem> GetAll()
    {
        var items = new List<StockItem>();
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            
            SELECT Id, Name, Quantity,Unit, Price
            FROM StockItems
            Order BY Id
            
            """;
            

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(new StockItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Quantity = reader.GetDecimal(2),
                Unit = reader.GetString(3),
                Price = reader.GetDecimal(4)
            });
        }

        return items;
    }

    public StockItem? GetById(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            SELECT Id, Name, Quantity, Unit, Price
            FROM StockItems
            WHERE Id = @Id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new StockItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Quantity = reader.GetDecimal(2),
                Unit = reader.GetString(3),
                Price = reader.GetDecimal(4)
            };
        }

        return null;

    }

    public bool ExistingByName(string name, int? excludeId = null)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        string sql;

        if (excludeId.HasValue)
        {
            sql = """

                SELECT COUNT(1)
                FROM StockItems
                WHERE Name = @Name
                AND Id <> @ExcludeId
                """;
        }
        else
        {
            sql = """
                select count(1)
                from stockitems
                where name = @Name
                """;
        }

        using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar,100)
            .Value = name;

        if (excludeId.HasValue)
        {
            command.Parameters.Add("@ExcludeId", System.Data.SqlDbType.Int)
                .Value = excludeId.Value;
        }
        var count = (int)command.ExecuteScalar()!;
        return count > 0;
    }


    public void Add(StockItem item)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            INSERT INTO StockItems (Name, Quantity, Unit, Price)
            VALUES (@Name, @Quantity, @Unit, @Price)
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", item.Name);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@Unit", item.Unit);
        command.Parameters.AddWithValue("@Price", item.Price);

        command.ExecuteNonQuery();
    }

    public void Update(StockItem item)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            UPDATE StockItems
            SET
                Name = @Name,
                Quantity = @Quantity,
                Unit = @Unit,
                Price = @Price
            WHERE Id = @Id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@Name", item.Name);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@Unit", item.Unit);
        command.Parameters.AddWithValue("@Price", item.Price);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            DELETE FROM StockItems WHERE Id = @Id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }
}
