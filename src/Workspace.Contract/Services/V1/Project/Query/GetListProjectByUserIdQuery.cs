namespace Workspace.Contract
{
    public class GetListProjectByUserIdQuery : IQuery<List<ProjectResponse>>
    {
        public string? SearchTerm { get; set; }
    }
}
