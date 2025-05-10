using Dapper;

namespace Movies.Application.Database;

public class DbInitializer(IDbConnectionFactory connectionFactory)
{
    public async Task InitializeAsync()
    {
        using var connection = await connectionFactory.CreateConnectionAsync();
        
        await connection.ExecuteAsync($"""
                                           CREATE TABLE IF NOT EXISTS movies (
                                               id UUID PRIMARY KEY,
                                               slug TEXT NOT NULL,
                                               title TEXT NOT NULL,
                                               yearofrelease integer NOT NULL
                                           );
                                       """);
        await connection.ExecuteAsync($"""
                                      CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS movies_slug_idx 
                                      ON movies 
                                      using btree(slug);
                                      """);
        
        await connection.ExecuteAsync($"""
                                       CREATE TABLE IF NOT EXISTS genres(
                                           movieId UUID references movies(id) ON DELETE CASCADE,
                                           name TEXT NOT NULL
                                       );
                                       """);
    }
}