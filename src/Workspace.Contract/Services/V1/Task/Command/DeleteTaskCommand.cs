namespace Workspace.Contract
{
    public class DeleteTaskCommand : ICommand
    {
        public Guid Id { get; set; }

        public DeleteTaskCommand(Guid id)
        {
            Id = id;
        }
    }
}
