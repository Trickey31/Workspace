namespace Workspace.Contract
{
    public class CreateTaskCommand : TaskCommand
    {
        public Guid Type { get; set; }
    }
}
