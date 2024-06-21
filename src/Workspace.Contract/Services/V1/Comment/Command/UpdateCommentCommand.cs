namespace Workspace.Contract
{
    public class UpdateCommentCommand : ICommand
    {
        public Guid Id { get; set; }

        public string Content { get; set; }
    }
}
