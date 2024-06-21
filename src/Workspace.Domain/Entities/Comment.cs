namespace Workspace.Domain
{
    public class Comment : DomainEntity<Guid>
    {
        public Guid UserId { get; set; }

        public Guid TaskId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
