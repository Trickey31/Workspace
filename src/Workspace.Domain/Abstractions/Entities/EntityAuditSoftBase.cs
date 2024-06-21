namespace Workspace.Domain
{
    public abstract class EntityAuditSoftBase<T> : DomainEntity<T>, ISoftDelete
    {
        public int IsDelete { get; set; }
    }
}
