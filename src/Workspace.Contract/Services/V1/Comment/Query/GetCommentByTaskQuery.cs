namespace Workspace.Contract
{
    public class GetCommentByTaskQuery : IQuery<List<CommentResponse>>
    {
        public Guid TaskId { get; set; }
    }
}
