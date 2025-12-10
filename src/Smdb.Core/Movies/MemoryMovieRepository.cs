using Smdb.Core.Db;
using Shared.Results;

namespace Smdb.Core.Movies;

public class MemoryMovieRepository : IMovieRepository
{
    public Task<PagedResult<Movie>> GetAllAsync(int page, int size)
    {
        var db = MemoryDatabase.Instance;
        var total = db.Movies.Count;
        var items = db.Movies
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        return Task.FromResult(new PagedResult<Movie>(items, page, size, total));
    }

    public Task<Result<Movie>> GetByIdAsync(int id)
    {
        var db = MemoryDatabase.Instance;
        var movie = db.Movies.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(movie == null
            ? Result<Movie>.CreateFailure("Movie not found")
            : Result<Movie>.CreateSuccess(movie));
    }

    public Task<Result<Movie>> CreateAsync(Movie movie)
    {
        var db = MemoryDatabase.Instance;
        movie.Id = db.NextMovieId();
        db.Movies.Add(movie);
        return Task.FromResult(Result<Movie>.CreateSuccess(movie));
    }

    public Task<Result<Movie>> UpdateAsync(Movie movie)
    {
        var db = MemoryDatabase.Instance;
        var existing = db.Movies.FirstOrDefault(m => m.Id == movie.Id);
        if (existing == null)
            return Task.FromResult(Result<Movie>.CreateFailure("Movie not found"));

        existing.Title = movie.Title;
        existing.Year = movie.Year;
        existing.Description = movie.Description;
        existing.Genre = movie.Genre;
        existing.Rating = movie.Rating;
        return Task.FromResult(Result<Movie>.CreateSuccess(existing));
    }

    public Task<Result<bool>> DeleteAsync(int id)
    {
        var db = MemoryDatabase.Instance;
        var movie = db.Movies.FirstOrDefault(m => m.Id == id);
        if (movie == null)
            return Task.FromResult(Result<bool>.CreateFailure("Movie not found"));

        db.Movies.Remove(movie);
        return Task.FromResult(Result<bool>.CreateSuccess(true));
    }
}
