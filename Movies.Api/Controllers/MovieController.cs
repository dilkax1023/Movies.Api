using Microsoft.AspNetCore.Mvc;
using Movies.Api.Constants;
using Movies.Api.Mappings;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MovieController(IMovieService movieService) : ControllerBase
{
     [HttpPost(Endpoints.Movies.Create)]
     public async Task<IActionResult> Create([FromBody]CreateMovieRequest request, CancellationToken token)
     {
          var movie = request.MapToMovie();
          var result = await movieService.CreateAsync(movie, token);
          return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id}, movie);
     }

     [HttpGet(Endpoints.Movies.GetAll)]
     public async Task<IActionResult> GetAll(CancellationToken token)
     {
          var movies = await movieService.GetAllAsync(token);
          return Ok(movies.MapToMoviesResponse());
     }

     [HttpGet(Endpoints.Movies.Get)]
     public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
     {
          
          var movie = Guid.TryParse(idOrSlug, out var id)
               ? await movieService.GetByIdAsync(id, token)
               : await movieService.GetBySlugAsync(idOrSlug, token);

          if (movie is null)
          {
               return NotFound();
          }
          
          return Ok(movie.MapToMovieResponse());
     }

     [HttpPut(Endpoints.Movies.Update)]
     public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
     {
          var movie = request.MapToMovie(id);
          var result = await movieService.UpdateAsync(movie, token);

          if (result is null )
          {
               return NotFound();
          }
          return Ok(movie.MapToMovieResponse());
     }

     [HttpDelete(Endpoints.Movies.Delete)]
     public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
     {
          var result = await movieService.DeleteByIdAsync(id, token);

          if (!result)
          {
               return NotFound();
          }
          return Ok();
     }
}