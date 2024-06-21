namespace Workspace.Contract
{
    public record GetProjectByIdQuery : IQuery<ProjectResponse>
    {
        public Guid Id { get; set; }

        public GetProjectByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
