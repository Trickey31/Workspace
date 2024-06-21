namespace Workspace.Contract
{
    public class GetRoleInProjectQuery : IQuery<RoleResponse>
    {
        public Guid ProjectId { get; set; }
    }
}
