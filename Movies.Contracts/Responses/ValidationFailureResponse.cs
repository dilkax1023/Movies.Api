namespace Movies.Contracts.Responses;

public class ValidationFailureResponse
{
    public required IEnumerable<ValidationResponse> Errors { get; init; } = null!;
}

public class ValidationResponse
{
    public required string PropertyName { get; init; } = null!;
    public required string ErrorMessage { get; init; } = null!;
}