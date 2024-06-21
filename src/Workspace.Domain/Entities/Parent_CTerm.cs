namespace Workspace.Domain
{
    public class Parent_CTerm : EntityAuditBase<Guid>
    {
        public Guid ParentId { get; set; }

        public Guid TypeId { get; set; }
    }
}
