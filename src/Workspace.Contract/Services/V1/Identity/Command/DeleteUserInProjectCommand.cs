namespace Workspace.Contract
{
    public class DeleteUserInProjectCommand : ICommand
    {
        public Guid Id { get; set; }

        public Guid ProjectId {  get; set; }
    }
}
