namespace Workspace.Contract
{
    public class ProjectCommandResponse
    {
        public Guid Id { get; set; }

        public string Slug { get; set; }

        public ProjectCommandResponse(Guid id, string slug)
        {
            Id = id;
            Slug = slug;
        }
    }
}
