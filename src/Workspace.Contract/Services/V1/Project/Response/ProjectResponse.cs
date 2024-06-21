namespace Workspace.Contract
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string Slug { get; set; }

        public Guid? LeaderId { get; set; }

        public string LeaderName { get; set; }

        public string? ImgLink { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
