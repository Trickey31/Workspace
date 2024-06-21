namespace Workspace.Domain
{
    public class Users_Projects : EntityAuditBase<Guid>
    {
        public Guid UserId { get; set; }

        public Guid ProjectId { get; set; }

        public int RoleId { get; set; }
    }
}
