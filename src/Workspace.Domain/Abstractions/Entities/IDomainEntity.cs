namespace Workspace.Domain
{
    public interface IDomainEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
