namespace Workspace.Domain
{
    public interface IEntityAuditBase<T> : IDomainEntity<T>, IAuditable
    {
    }
}
