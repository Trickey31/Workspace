
namespace Workspace.Domain
{
    public abstract class EntityAuditBase<T> : DomainEntity<T>, IEntityAuditBase<T>
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsDelete { get; set; }
    }
}
