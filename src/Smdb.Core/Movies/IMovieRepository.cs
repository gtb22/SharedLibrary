using Shared.Results;

namespace Smdb.Core.Movies;

public interface IMovieRepository
{
    Task<PagedResult<Movie>> GetAllAsync(int page, int size);
    Task<Result<Movie>> GetByIdAsync(int id);
    Task<Result<Movie>> CreateAsync(Movie movie);
    Task<Result<Movie>> UpdateAsync(Movie movie);
    Task<Result<bool>> DeleteAsync(int id);
}
