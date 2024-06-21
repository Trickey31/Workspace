namespace Workspace.Contract
{
    public class DeleteUserCommand : ICommand
    {
        public Guid Id { get; set; }

        public DeleteUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
