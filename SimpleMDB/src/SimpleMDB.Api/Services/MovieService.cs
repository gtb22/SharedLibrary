using SimpleMDB.Core;

namespace SimpleMDB.Api.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;

    public MovieService(IMovieRepository repository)
    {
        _repository = repository;
    }

    public PagedResult<Movie> ReadMovies(int page, int size)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 10;

        return _repository.ReadMovies(page, size);
    }

    public Result<Movie> ReadMovie(int id)
    {
        if (id <= 0)
            return Result<Movie>.Failure("Invalid movie ID", 400);

        return _repository.ReadMovie(id);
    }

    public Result<Movie> CreateMovie(Movie movie)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(movie.Title))
            return Result<Movie>.Failure("Title is required", 400);

        if (movie.Year < 1888 || movie.Year > DateTime.Now.Year + 1)
            return Result<Movie>.Failure("Invalid year", 400);

        if (movie.Rating < 0 || movie.Rating > 10)
            return Result<Movie>.Failure("Rating must be between 0 and 10", 400);

        // Business rule: Check for duplicates (simple check by title and year)
        var existing = _repository.ReadMovies(1, int.MaxValue).Items
            .FirstOrDefault(m => m.Title.Equals(movie.Title, StringComparison.OrdinalIgnoreCase) && m.Year == movie.Year);
        if (existing != null)
            return Result<Movie>.Failure("Movie with same title and year already exists", 409);

        return _repository.CreateMovie(movie);
    }

    public Result<Movie> UpdateMovie(Movie movie)
    {
        // Input validation
        if (movie.Id <= 0)
            return Result<Movie>.Failure("Invalid movie ID", 400);

        if (string.IsNullOrWhiteSpace(movie.Title))
            return Result<Movie>.Failure("Title is required", 400);

        if (movie.Year < 1888 || movie.Year > DateTime.Now.Year + 1)
            return Result<Movie>.Failure("Invalid year", 400);

        if (movie.Rating < 0 || movie.Rating > 10)
            return Result<Movie>.Failure("Rating must be between 0 and 10", 400);

        return _repository.UpdateMovie(movie);
    }

    public Result<bool> DeleteMovie(int id)
    {
        if (id <= 0)
            return Result<bool>.Failure("Invalid movie ID", 400);

        return _repository.DeleteMovie(id);
    }
}
