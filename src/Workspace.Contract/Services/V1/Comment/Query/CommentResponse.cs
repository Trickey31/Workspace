namespace Workspace.Contract
{
    public class CommentResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ImgLink { get; set; }

        public Guid TaskId { get; set; }

        public Guid UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
