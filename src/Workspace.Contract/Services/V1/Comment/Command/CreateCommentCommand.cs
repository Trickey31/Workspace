namespace Workspace.Contract
{
    public class CreateCommentCommand : ICommand
    {
        public Guid TaskId {  get; set; }

        public string Content { get; set; }
    }
}
