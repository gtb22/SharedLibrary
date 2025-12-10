using Shared.Results;

namespace Smdb.Core.Users;

public interface IUserRepository
{
    Task<Result<User>> GetByUsernameAsync(string username);
    Task<Result<User>> GetByIdAsync(int id);
    Task<Result<User>> CreateAsync(User user);
}
