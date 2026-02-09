namespace OAuthPractice.Contracts
{
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken ct);
    }
}
