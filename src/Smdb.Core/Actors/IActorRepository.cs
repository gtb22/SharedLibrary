using Shared.Results;

namespace Smdb.Core.Actors;

public interface IActorRepository
{
    Task<PagedResult<Actor>> GetAllAsync(int page, int size);
    Task<Result<Actor>> GetByIdAsync(int id);
    Task<Result<Actor>> CreateAsync(Actor actor);
    Task<Result<Actor>> UpdateAsync(Actor actor);
    Task<Result<bool>> DeleteAsync(int id);
}
