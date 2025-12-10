using Shared.Results;

namespace Smdb.Core.Actors;

public class ActorService
{
    private readonly IActorRepository _repository;

    public ActorService(IActorRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Actor>> GetAllAsync(int page, int size)
    {
        return await _repository.GetAllAsync(page, size);
    }

    public async Task<Result<Actor>> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Result<Actor>> CreateAsync(Actor actor)
    {
        if (string.IsNullOrWhiteSpace(actor.Name))
            return Result<Actor>.CreateFailure("Name is required");
        if (actor.BirthYear < 1800 || actor.BirthYear > DateTime.Now.Year)
            return Result<Actor>.CreateFailure("Invalid birth year");

        return await _repository.CreateAsync(actor);
    }

    public async Task<Result<Actor>> UpdateAsync(Actor actor)
    {
        if (string.IsNullOrWhiteSpace(actor.Name))
            return Result<Actor>.CreateFailure("Name is required");
        if (actor.BirthYear < 1800 || actor.BirthYear > DateTime.Now.Year)
            return Result<Actor>.CreateFailure("Invalid birth year");

        return await _repository.UpdateAsync(actor);
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
