using Smdb.Core.Db;
using Shared.Results;

namespace Smdb.Core.Actors;

public class MemoryActorRepository : IActorRepository
{
    public Task<PagedResult<Actor>> GetAllAsync(int page, int size)
    {
        var db = MemoryDatabase.Instance;
        var total = db.Actors.Count;
        var items = db.Actors
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        return Task.FromResult(new PagedResult<Actor>(items, page, size, total));
    }

    public Task<Result<Actor>> GetByIdAsync(int id)
    {
        var db = MemoryDatabase.Instance;
        var actor = db.Actors.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(actor == null
            ? Result<Actor>.CreateFailure("Actor not found")
            : Result<Actor>.CreateSuccess(actor));
    }

    public Task<Result<Actor>> CreateAsync(Actor actor)
    {
        var db = MemoryDatabase.Instance;
        actor.Id = db.NextActorId();
        db.Actors.Add(actor);
        return Task.FromResult(Result<Actor>.CreateSuccess(actor));
    }

    public Task<Result<Actor>> UpdateAsync(Actor actor)
    {
        var db = MemoryDatabase.Instance;
        var existing = db.Actors.FirstOrDefault(a => a.Id == actor.Id);
        if (existing == null)
            return Task.FromResult(Result<Actor>.CreateFailure("Actor not found"));

        existing.Name = actor.Name;
        existing.BirthYear = actor.BirthYear;
        existing.Bio = actor.Bio;
        return Task.FromResult(Result<Actor>.CreateSuccess(existing));
    }

    public Task<Result<bool>> DeleteAsync(int id)
    {
        var db = MemoryDatabase.Instance;
        var actor = db.Actors.FirstOrDefault(a => a.Id == id);
        if (actor == null)
            return Task.FromResult(Result<bool>.CreateFailure("Actor not found"));

        db.Actors.Remove(actor);
        return Task.FromResult(Result<bool>.CreateSuccess(true));
    }
}
