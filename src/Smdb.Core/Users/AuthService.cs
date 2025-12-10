using Shared.Results;

namespace Smdb.Core.Users;

public class AuthService
{
    private readonly IUserRepository _repository;

    public AuthService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<User>> LoginAsync(string username, string password)
    {
        var result = await _repository.GetByUsernameAsync(username);
        if (!result.Success)
            return Result<User>.CreateFailure("Invalid credentials");

        var user = result.Data!;
        var passwordHash = ComputeHash(password);
        
        if (user.PasswordHash != passwordHash)
            return Result<User>.CreateFailure("Invalid credentials");

        return Result<User>.CreateSuccess(user);
    }

    private static string ComputeHash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }
}
