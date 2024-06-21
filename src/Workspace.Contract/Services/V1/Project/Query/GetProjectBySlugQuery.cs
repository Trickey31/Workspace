namespace Workspace.Contract
{
    public record GetProjectBySlugQuery : IQuery<ProjectResponse>
    {
        public string Slug { get; set; }

        public GetProjectBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }
}
