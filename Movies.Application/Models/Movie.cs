using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }

    public string Slug => GetMovieSlug();

    public required int YearOfRelease { get; set; }

    public required List<string> Genres { get; init; } = [];

    private string GetMovieSlug()
    {
        var replaceSpecialChars = MyRegex();
        var titleWithoutSpecialChars = replaceSpecialChars.Replace(Title, "");

        var slug = titleWithoutSpecialChars
            .Replace(" ", "-")
            .ToLower();

        return $"{slug}-{YearOfRelease}";
    }

    [GeneratedRegex(@"[^a-zA-Z0-9\s]")]
    private static partial Regex MyRegex();

}