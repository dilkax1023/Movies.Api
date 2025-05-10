using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository) : IMovieService
{
    public Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken)
    {
        return movieRepository.CreateAsync(movie, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken cancellationToken)
    {
        var existingMovie = await movieRepository.GetByIdAsync(movie.Id, cancellationToken);
        if (existingMovie is null)
        {
            return null;
        }
        
        await movieRepository.UpdateAsync(movie, cancellationToken);
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return movieRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return movieRepository.GetBySlugAsync(slug, cancellationToken);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken)
    {
        return movieRepository.GetAllAsync(cancellationToken);
    }
}