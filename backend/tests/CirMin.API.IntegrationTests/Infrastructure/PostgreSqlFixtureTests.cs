using System.Data;
using Npgsql;

namespace CirMin.API.IntegrationTests.Infrastructure;

[Collection(PostgreSqlCollection.Name)]
public class PostgreSqlFixtureTests
{
    private readonly PostgreSqlFixture _database;

    public PostgreSqlFixtureTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Connection_WhenContainerStarted_CanBeOpened()
    {
        await using var connection = new NpgsqlConnection(_database.ConnectionString);

        await connection.OpenAsync();

        Assert.Equal(ConnectionState.Open, connection.State);
    }
}