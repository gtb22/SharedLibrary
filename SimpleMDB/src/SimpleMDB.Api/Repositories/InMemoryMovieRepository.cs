using SimpleMDB.Core;

namespace SimpleMDB.Api.Repositories;

public class InMemoryMovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = new();
    private int _nextId = 1;

    public PagedResult<Movie> ReadMovies(int page, int size)
    {
        var items = _movies.Skip((page - 1) * size).Take(size).ToList();
        return new PagedResult<Movie>
        {
            Items = items,
            TotalCount = _movies.Count,
            Page = page,
            PageSize = size
        };
    }

    public Result<Movie> ReadMovie(int id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        return movie != null ? Result<Movie>.Success(movie) : Result<Movie>.Failure("Movie not found", 404);
    }

    public Result<Movie> CreateMovie(Movie movie)
    {
        movie.Id = _nextId++;
        _movies.Add(movie);
        return Result<Movie>.Success(movie);
    }

    public Result<Movie> UpdateMovie(Movie movie)
    {
        var existing = _movies.FirstOrDefault(m => m.Id == movie.Id);
        if (existing == null)
            return Result<Movie>.Failure("Movie not found", 404);

        existing.Title = movie.Title;
        existing.Year = movie.Year;
        existing.Rating = movie.Rating;
        existing.Description = movie.Description;
        return Result<Movie>.Success(existing);
    }

    public Result<bool> DeleteMovie(int id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        if (movie == null)
            return Result<bool>.Failure("Movie not found", 404);

        _movies.Remove(movie);
        return Result<bool>.Success(true);
    }
}
