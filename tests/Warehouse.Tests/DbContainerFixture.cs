using System.Reflection;
using DbUp;
using Respawn;
using Testcontainers.PostgreSql;

namespace Warehouse.Tests;

public class DbContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer  _postgresContainer;
    private readonly Checkpoint _checkpoint;
    public string ConnectionString { get; private set; }

    public DbContainerFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("warehouse.tests")
            .WithUsername("postgres")
            .WithPassword("password")
            .WithPortBinding(5432, 5432)
            .Build();

        _checkpoint = new Checkpoint
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = new[] { "__EFMigrationsHistory" }
        };
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        ApplyMigrations();
    }

    private void ApplyMigrations()
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly()) 
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception("Database migration failed", result.Error);
        }
    }

    public async Task ResetDatabaseAsync()
    {
        await _checkpoint.Reset(ConnectionString);
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
    }
}

