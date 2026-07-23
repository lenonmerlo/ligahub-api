using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace LigaHub.Infrastructure.IntegrationTests.Database;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container =
        new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString
    {
        get
        {
            var builder = new SqlConnectionStringBuilder(
                _container.GetConnectionString())
            {
                InitialCatalog = "LigaHubTests"
            };

            return builder.ConnectionString;
        }
    }

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }
}
