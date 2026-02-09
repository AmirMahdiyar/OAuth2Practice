using OAuthPractice.Contracts;

namespace OAuthPractice.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OAuthDbContext _context;

        public UnitOfWork(OAuthDbContext context)
        {
            _context = context;
        }

        public async Task Commit(CancellationToken ct)
        {
            await _context.SaveChangesAsync();
        }
    }
}
