namespace OAuthPractice.Entity.Base
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; }
    }
}

