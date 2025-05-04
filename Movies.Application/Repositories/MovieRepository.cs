using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];
    public Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken)
    {
        var existingMovieIndex = _movies.FindIndex(m => m.Id == movie.Id);
        if (existingMovieIndex == -1)
        {
            return Task.FromResult(false);
        }
        _movies[existingMovieIndex] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = _movies.RemoveAll(m => m.Id == id);
        
        var hasDeleted = result > 0;
        
        return Task.FromResult(hasDeleted);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        
        return Task.FromResult(movie);
    }
    
    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var movie = _movies.FirstOrDefault(m => m.Slug == slug);
        
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_movies.AsEnumerable());
    }
}