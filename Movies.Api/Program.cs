using Movies.Api;
using Movies.Application.Database;
using Movies.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddApplication();
services.AddDatabase(configuration["Database:ConnectionString"]!);
    
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseValidation();
app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();

await dbInitializer.InitializeAsync();

app.Run();