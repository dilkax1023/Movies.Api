using FluentValidation;
using Movies.Contracts.Responses;

namespace Movies.Api.Middlewares;

public class ValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var validationFailuresResponse = new ValidationFailureResponse()
            {
                Errors = e.Errors.Select(err => new ValidationResponse()
                {
                    PropertyName = err.PropertyName,
                    ErrorMessage = err.ErrorMessage
                })
            };
            
            await context.Response.WriteAsJsonAsync(validationFailuresResponse);
        }
    }
}