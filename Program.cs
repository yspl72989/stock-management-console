using StockApi.Data;
using StockApi.Repositories;
using StockApi.Services;

var builder = WebApplication.CreateBuilder(args);

string stockConnectionString =
    builder.Configuration.GetConnectionString("StockDb")
    ?? throw new InvalidOperationException(
        "StockDb connection string is missing.");

string masterConnectionString =
    builder.Configuration.GetConnectionString("MasterDb")
    ?? throw new InvalidOperationException(
        "MasterDb connection string is missing.");

var initializer = new DatabaseInitializer(
    stockConnectionString,
    masterConnectionString);

initializer.Initialize();

builder.Services.AddScoped<StockDb>(_ => new StockDb(stockConnectionString));
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program { }
