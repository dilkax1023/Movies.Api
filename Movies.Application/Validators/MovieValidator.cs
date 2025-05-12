using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;
    
    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        
        RuleFor(m => m.Id).NotEmpty()
            .WithMessage("Id is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("Id must be a valid GUID.");
        
        RuleFor(m => m.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .Length(1, 10)
            .WithMessage("Title must be between 1 and 100 characters.")
            .Matches(@"^[a-zA-Z\s]+$")
            .WithMessage("Title can only contain letters, numbers, and spaces.");

        RuleFor(m => m.YearOfRelease)
            .NotEmpty()
            .WithMessage("Release date is required.")
            .LessThan(DateTime.UtcNow.Year)
            .WithMessage("Release date must be in the past.");

        RuleFor(m => m.Genres)
            .NotEmpty()
            .WithMessage("Genre is required.");

        RuleFor(m => m.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("The movie with this slug is already exists.");
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug, cancellationToken);
        if (existingMovie is not null)
        {
            return existingMovie.Id == movie.Id;
        }
        return existingMovie is null;
    }
}