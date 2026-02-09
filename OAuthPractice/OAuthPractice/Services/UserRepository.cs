using Microsoft.EntityFrameworkCore;
using OAuthPractice.Contracts;
using OAuthPractice.Database;
using OAuthPractice.Entity;

namespace OAuthPractice.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly OAuthDbContext _context;

        public UserRepository(OAuthDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public async Task<User> GetUser(string gmail, CancellationToken ct)
        {
            return await _context.Users
                        .Include(x => x.Authentications)
                        .Include(x => x.ContactInformation)
                        .SingleOrDefaultAsync(x => x.ContactInformation.Gmail == gmail);
        }

        public async Task<User> GetUser(Guid id, CancellationToken ct)
        {
            return await _context.Users
                        .Include(x => x.Authentications)
                        .Include(x => x.ContactInformation)
                        .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetUserByUsername(string username, CancellationToken ct)
        {
            return await _context.Users
                        .Include(x => x.Authentications)
                        .Include(x => x.ContactInformation)
                        .SingleOrDefaultAsync(x => x.Username == username);
        }
    }
}
