using Microsoft.Data.SqlClient;
using StockCli.Data;
using StockCli.Models;

namespace StockCli.Repositories;

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

        const string sql = "SELECT Id, Name, Quantity, Price FROM StockItems";

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(new StockItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Quantity = reader.GetInt32(2),
                Price = reader.GetDecimal(3)
            });
        }

        return items;
    }

    public StockItem? GetById(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = "SELECT Id, Name, Quantity, Price FROM StockItems WHERE Id = @Id";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new StockItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Quantity = reader.GetInt32(2),
                Price = reader.GetDecimal(3)
            };
        }

        return null;
    }

    public void Add(StockItem item)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = "INSERT INTO StockItems (Name, Quantity, Price) VALUES (@Name, @Quantity, @Price)";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", item.Name);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@Price", item.Price);

        command.ExecuteNonQuery();
    }

    public void Update(StockItem item)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            UPDATE StockItems
            SET Name = @Name, Quantity = @Quantity, Price = @Price
            WHERE Id = @Id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@Name", item.Name);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@Price", item.Price);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = "DELETE FROM StockItems WHERE Id = @Id";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }
}
