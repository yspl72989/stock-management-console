using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StockApi.Test.IntegrationTests.Fixtures;

public sealed class WebApplicationFactoryFixture : IDisposable
{
    //the scope belong to the WebApplicationFactoryFixture
    private readonly IServiceScope _serviceScope;

    internal WebApplicationFactory<Program> WebApplicationFactory { get; private set; }

    internal HttpClient HttpClient { get; }

    /// <summary>
    /// Scoped service provider for resolving app services (e.g. IStockService).
    /// </summary>
    internal IServiceProvider Services { get; }

    public WebApplicationFactoryFixture()
    {
        WebApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddJsonFile("appsettings.json", optional: false);
                    configBuilder.AddEnvironmentVariables();
                });
            });

        HttpClient = WebApplicationFactory.CreateClient();
        //library method 
        _serviceScope = WebApplicationFactory.Services.CreateScope();
        Services = _serviceScope.ServiceProvider;
        
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
        HttpClient.Dispose();
        WebApplicationFactory.Dispose();
    }
}
