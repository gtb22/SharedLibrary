using SimpleMDB.Core;

namespace SimpleMDB.Api.Controllers;

public static class MoviesController
{
    public static void MapMoviesRoutes(this WebApplication app)
    {
        app.MapGet("/api/v1/movies", (IMovieService service, int? page, int? size) =>
        {
            page ??= 1;
            size ??= 10;

            var result = service.ReadMovies(page.Value, size.Value);
            return Results.Ok(result);
        });

        app.MapGet("/api/v1/movies/{id:int}", (IMovieService service, int id) =>
        {
            var result = service.ReadMovie(id);
            if (!result.IsSuccess)
                return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);

            return Results.Ok(result.Value);
        });

        app.MapPost("/api/v1/movies", async (IMovieService service, Movie movie) =>
        {
            if (movie == null)
                return Results.BadRequest(new { error = "Invalid movie data" });

            var result = service.CreateMovie(movie);
            if (!result.IsSuccess)
                return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);

            return Results.Created($"/api/v1/movies/{result.Value!.Id}", result.Value);
        });

        app.MapPut("/api/v1/movies/{id:int}", async (IMovieService service, int id, Movie movie) =>
        {
            if (movie == null)
                return Results.BadRequest(new { error = "Invalid movie data" });

            movie.Id = id;
            var result = service.UpdateMovie(movie);
            if (!result.IsSuccess)
                return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);

            return Results.Ok(result.Value);
        });

        app.MapDelete("/api/v1/movies/{id:int}", (IMovieService service, int id) =>
        {
            var result = service.DeleteMovie(id);
            if (!result.IsSuccess)
                return Results.Json(new { error = result.Error }, statusCode: result.StatusCode);

            return Results.NoContent();
        });
    }
}
