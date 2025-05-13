using System.Data;
using Npgsql;

namespace Movies.Application.Database;

public class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

public interface IDbConnectionFactory 
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}