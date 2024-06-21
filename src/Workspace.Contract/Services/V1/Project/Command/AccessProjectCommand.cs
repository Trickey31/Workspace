namespace Workspace.Contract
{
    public class AccessProjectCommand : ICommand
    {
        public string Email { get; set; }

        public Guid ProjectId { get; set; }

        public int? RoleId {  get; set; }
    }
}
