namespace Workspace.Contract
{
    public class DeleteProjectCommand : ICommand
    {
        public Guid Id { get; set; }

        public DeleteProjectCommand(Guid id)
        {
            Id = id;
        }
    }
}
