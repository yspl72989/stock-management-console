using System.Data;
// CommandType.StoredProcedure from system.data namespace
using Microsoft.Data.SqlClient;
using StockApi.Constants;
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

        using var command = CreateProcedureCommand(connection, StockItemProcedures.GetAll);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(MapStockItem(reader));
        }

        return items;
    }

    public StockItem? GetById(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockItemProcedures.GetById);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        using var reader = command.ExecuteReader();

        return reader.Read() ? MapStockItem(reader) : null;
    }

    public List<StockItem> CheckUpdates(DateTime lastModifiedDate)
    {
        var items = new List<StockItem>();

        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockItemProcedures.CheckUpdates);
        command.Parameters.Add("@LastModifiedDate", SqlDbType.DateTime2).Value = lastModifiedDate;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(MapStockItem(reader));
        }

        return items;
    }

    public bool ExistingByName(string name, int? excludeId = null)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockItemProcedures.ExistingByName);
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
        command.Parameters.Add("@ExcludeId", SqlDbType.Int).Value =
            excludeId.HasValue ? excludeId.Value : DBNull.Value;

        var count = (int)command.ExecuteScalar()!;
        return count > 0;
    }

    public void Add(StockItem item)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockItemProcedures.Insert);
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = item.Name;
        command.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = item.Quantity;
        command.Parameters.Add("@Unit", SqlDbType.NVarChar, 20).Value = item.Unit;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = item.Price;
        command.Parameters.Add("@LastModifiedDate", SqlDbType.DateTime2).Value = item.LastModifiedDate;

        command.ExecuteNonQuery();
    }

    public void Update(StockItem item)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockItemProcedures.Update);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = item.Id;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = item.Name;
        command.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = item.Quantity;
        command.Parameters.Add("@Unit", SqlDbType.NVarChar, 20).Value = item.Unit;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = item.Price;
        command.Parameters.Add("@LastModifiedDate", SqlDbType.DateTime2).Value = item.LastModifiedDate;

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var command = CreateProcedureCommand(connection, StockItemProcedures.Delete);
        ///  → SqlCommand named "dbo.uspStockItem_Delete", CommandType = StoredProcedure
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        // → pass @Id into the sproc

        command.ExecuteNonQuery();
        //  → run the sproc (no rows returned, just deletes)
    }

// create a private method to create a command with the procedure name
// and the connection. Also The helper avoids repeating this in all 7 methods:
//var command = new SqlCommand(procedureName, connection);
//command.CommandType = CommandType.StoredProcedure;

   private static SqlCommand CreateProcedureCommand(SqlConnection connection, string procedureName)
    {
        var command = new SqlCommand(procedureName, connection)
        {
            //CommandType tells SQL Server how to interpret the first argument
            //Without CommandType = StoredProcedure, ADO.NET treats "dbo.uspStockItem_Delete" as SQL text, not a procedure call → error or wrong behaviour.
            CommandType = CommandType.StoredProcedure
            //CommandType.StoredProcedure — tells ADO.NET the string is a procedure name, not raw SQL
        };
        return command;
    }
 

//converts one SqlDataReader row into a StockItem (DRY mapping)
    private static StockItem MapStockItem(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
        Unit = reader.GetString(reader.GetOrdinal("Unit")),
        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
        LastModifiedDate = reader.GetDateTime(reader.GetOrdinal("LastModifiedDate"))
    };
}
