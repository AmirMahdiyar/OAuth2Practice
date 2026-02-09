using OAuthPractice.Entity;

namespace OAuthPractice.Contracts
{
    public interface IUserRepository
    {
        void AddUser(User user);
        Task<User> GetUser(string gmail, CancellationToken ct);
        Task<User> GetUser(Guid id, CancellationToken ct);
        Task<User> GetUserByUsername(string username, CancellationToken ct);
    }
}
