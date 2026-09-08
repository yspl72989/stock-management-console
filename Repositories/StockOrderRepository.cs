using System.Data;
using Microsoft.Data.SqlClient;
using StockApi.Constants;
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

        using var command = CreateProcedureCommand(connection, StockOrderProcedures.GetAll);
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

        using var command = CreateProcedureCommand(connection, StockOrderProcedures.GetById);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        using var reader = command.ExecuteReader();

        return reader.Read() ? MapOrder(reader) : null;
    }

    public StockOrder? GetByName(string name)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockOrderProcedures.GetByName);
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;

        using var reader = command.ExecuteReader();

        return reader.Read() ? MapOrder(reader) : null;
    }

    public void Create(StockOrder order)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockOrderProcedures.Insert);
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = order.Name;
        command.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = order.Quantity;
        command.Parameters.Add("@Unit", SqlDbType.NVarChar, 20).Value = order.Unit;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = order.Price;
        command.Parameters.Add("@LastModifiedDate", SqlDbType.DateTime2).Value = order.LastModifiedDate;
        command.Parameters.Add("@Invoice", SqlDbType.NVarChar, 10).Value = order.Invoice;

        command.ExecuteNonQuery();
    }

    public void Update(StockOrder order)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockOrderProcedures.Update);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = order.Id;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = order.Name;
        command.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = order.Quantity;
        command.Parameters.Add("@Unit", SqlDbType.NVarChar, 20).Value = order.Unit;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = order.Price;
        command.Parameters.Add("@LastModifiedDate", SqlDbType.DateTime2).Value = order.LastModifiedDate;
        command.Parameters.Add("@Invoice", SqlDbType.NVarChar, 10).Value = order.Invoice;

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockOrderProcedures.Delete);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        command.ExecuteNonQuery();
    }

    private static SqlCommand CreateProcedureCommand(SqlConnection connection, string procedureName)
    {
        var command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        return command;
    }

    private static StockOrder MapOrder(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
        Unit = reader.GetString(reader.GetOrdinal("Unit")),
        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
        LastModifiedDate = reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")),
        Invoice = reader.GetString(reader.GetOrdinal("Invoice"))
    };
}
