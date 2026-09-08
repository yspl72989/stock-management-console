using Microsoft.Data.SqlClient;
using StockApi.Data;
using StockApi.Models;

namespace StockApi.Repositories;

public class StockOrderRepository : IStockOrderRepository
{
    private readonly StockDb _db;

    public StockOrderRepository(StockDb db)
    {
        _db = db;
    }

    public List<StockOrder> GetAll()
    {
        var orders = new List<StockOrder>();
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate, Invoice
            FROM StockOrder
            ORDER BY Id
            """;

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            orders.Add(MapOrder(reader));
        }

        return orders;
    }

    public StockOrder? GetById(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate, Invoice
            FROM StockOrder
            WHERE Id = @Id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();

        return reader.Read() ? MapOrder(reader) : null;
    }

    public StockOrder? GetByName(string name)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            SELECT Id, Name, Quantity, Unit, Price, LastModifiedDate, Invoice
            FROM StockOrder
            WHERE Name = @Name
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", name);

        using var reader = command.ExecuteReader();

        return reader.Read() ? MapOrder(reader) : null;
    }

    public void Create(StockOrder order)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            INSERT INTO StockOrder (Name, Quantity, Unit, Price, LastModifiedDate, Invoice)
            VALUES (@Name, @Quantity, @Unit, @Price, @LastModifiedDate, @Invoice)
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", order.Name);
        command.Parameters.AddWithValue("@Quantity", order.Quantity);
        command.Parameters.AddWithValue("@Unit", order.Unit);
        command.Parameters.AddWithValue("@Price", order.Price);
        command.Parameters.AddWithValue("@LastModifiedDate", order.LastModifiedDate);
        command.Parameters.AddWithValue("@Invoice", order.Invoice);

        command.ExecuteNonQuery();
    }

    public void Update(StockOrder order)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            UPDATE StockOrder
            SET
                Name = @Name,
                Quantity = @Quantity,
                Unit = @Unit,
                Price = @Price,
                LastModifiedDate = @LastModifiedDate,
                Invoice = @Invoice
            WHERE Id = @Id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", order.Id);
        command.Parameters.AddWithValue("@Name", order.Name);
        command.Parameters.AddWithValue("@Quantity", order.Quantity);
        command.Parameters.AddWithValue("@Unit", order.Unit);
        command.Parameters.AddWithValue("@Price", order.Price);
        command.Parameters.AddWithValue("@LastModifiedDate", order.LastModifiedDate);
        command.Parameters.AddWithValue("@Invoice", order.Invoice);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        const string sql = """
            DELETE FROM StockOrder WHERE Id = @Id
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }

    private static StockOrder MapOrder(SqlDataReader reader)
    {
        return new StockOrder
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Quantity = reader.GetDecimal(2),
            Unit = reader.GetString(3),
            Price = reader.GetDecimal(4),
            LastModifiedDate = reader.GetDateTime(5),
            Invoice = reader.GetString(6)
        };
    }
}
