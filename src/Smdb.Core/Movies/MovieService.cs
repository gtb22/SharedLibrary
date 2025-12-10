using Shared.Results;

namespace Smdb.Core.Movies;

public class MovieService
{
    private readonly IMovieRepository _repository;

    public MovieService(IMovieRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Movie>> GetAllAsync(int page, int size)
    {
        return await _repository.GetAllAsync(page, size);
    }

    public async Task<Result<Movie>> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Result<Movie>> CreateAsync(Movie movie)
    {
        if (string.IsNullOrWhiteSpace(movie.Title))
            return Result<Movie>.CreateFailure("Title is required");
        if (movie.Year < 1800 || movie.Year > DateTime.Now.Year + 5)
            return Result<Movie>.CreateFailure("Invalid year");

        return await _repository.CreateAsync(movie);
    }

    public async Task<Result<Movie>> UpdateAsync(Movie movie)
    {
        if (string.IsNullOrWhiteSpace(movie.Title))
            return Result<Movie>.CreateFailure("Title is required");
        if (movie.Year < 1800 || movie.Year > DateTime.Now.Year + 5)
            return Result<Movie>.CreateFailure("Invalid year");

        return await _repository.UpdateAsync(movie);
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
