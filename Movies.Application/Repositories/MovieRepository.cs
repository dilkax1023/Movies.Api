using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        
        var result = await connection.ExecuteAsync(new CommandDefinition($"""
                                                                          insert into movies (id, title, slug, yearofrelease)
                                                                          values (@Id, @Title, @Slug, @YearOfRelease)
                                                                          """, movie, cancellationToken: cancellationToken));
        if (result <= 0) return false;
        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition($"""
                                                                 insert into genres (movieId, name)
                                                                 values(@MovieId, @Name)
                                                                 """, new
            {
                MovieId = movie.Id,
                Name = genre
            }, cancellationToken: cancellationToken));
        }
        
        transaction.Commit();
        return true;
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition(
            $"""
                delete from genres
                where movieId = @Id
            """, new { Id = movie.Id }, cancellationToken: cancellationToken));

        foreach (var genre in movie.Genres)
        {
           await connection.ExecuteAsync(new CommandDefinition($"""
                                                                insert into genres (movieId, name)
                                                                values(@MovieId, @Name)
                                                                """, new {MovieId = movie.Id, Name = genre}, cancellationToken: cancellationToken));
        }
        
        var result = await connection.ExecuteAsync(new CommandDefinition($"""
                                                                         update movies
                                                                            set title = @Title,
                                                                                slug = @Slug,
                                                                                yearofrelease = @YearOfRelease
                                                                            where id = @Id
                                                                         """, movie, cancellationToken: cancellationToken));


        transaction.Commit();
        
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var result = await connection.ExecuteAsync(new CommandDefinition($"""
                                                                         delete from movies
                                                                         where id = @Id
                                                                         """, new { Id = id }, cancellationToken: cancellationToken));
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(new CommandDefinition($"""
                                                                                             select * from movies
                                                                                             where id = @Id
                                                                                             """, new { Id = id }, cancellationToken: cancellationToken));
        if (movie is null) return null;
        var genres = await connection.QueryAsync<string>(new CommandDefinition($"""
                                                                                  select name from genres
                                                                                  where movieId = @Id
                                                                                  """, new { Id = id }, cancellationToken: cancellationToken));
        movie.Genres.AddRange(genres);
        return movie;  
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(new CommandDefinition($"""
                                                                                              select * from movies
                                                                                              where slug = @Slug
                                                                                              """, new { Slug = slug }, cancellationToken: cancellationToken));
        if (movie is null) return null;
        var genres = await connection.QueryAsync<string>(new CommandDefinition($"""
                                                                                   select name from genres
                                                                                   where movieId = @Id
                                                                                   """, new { Id = movie.Id }, cancellationToken: cancellationToken));
        movie.Genres.AddRange(genres);
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync(new CommandDefinition($"""
                                                                                   select m.*, string_agg(g.name, ',') as genres
                                                                                      from movies m left join genres g
                                                                                      on m.id = g.movieId
                                                                                      group by id
                                                                                   """, cancellationToken: cancellationToken));
        return result.Select(x => new Movie 
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition($"""
                                                                    select exists(
                                                                        select 1 from movies
                                                                        where id = @Id
                                                                    )
                                                                    """, new { Id = id }, cancellationToken: cancellationToken));
    }
}