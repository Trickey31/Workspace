namespace Workspace.Contract
{
    public class GetProjectsQuery : IQuery<List<ProjectResponse>>
    {
        public string? KeyWord { get; set; }
    }
}
