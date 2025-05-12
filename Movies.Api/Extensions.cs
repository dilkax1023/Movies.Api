using Movies.Api.Middlewares;

namespace Movies.Api;

public static class Extensions
{
    public static IApplicationBuilder UseValidation(this IApplicationBuilder app){
        return app.UseMiddleware<ValidationMiddleware>();
    }
}
