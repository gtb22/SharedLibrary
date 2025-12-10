using Smdb.Core.Db;
using Shared.Results;

namespace Smdb.Core.Users;

public class MemoryUserRepository : IUserRepository
{
    public Task<Result<User>> GetByUsernameAsync(string username)
    {
        var db = MemoryDatabase.Instance;
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        return Task.FromResult(user == null
            ? Result<User>.CreateFailure("User not found")
            : Result<User>.CreateSuccess(user));
    }

    public Task<Result<User>> GetByIdAsync(int id)
    {
        var db = MemoryDatabase.Instance;
        var user = db.Users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user == null
            ? Result<User>.CreateFailure("User not found")
            : Result<User>.CreateSuccess(user));
    }

    public Task<Result<User>> CreateAsync(User user)
    {
        var db = MemoryDatabase.Instance;
        user.Id = db.NextUserId();
        db.Users.Add(user);
        return Task.FromResult(Result<User>.CreateSuccess(user));
    }
}
