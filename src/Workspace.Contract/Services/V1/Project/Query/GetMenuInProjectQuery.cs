namespace Workspace.Contract
{
    public class GetMenuInProjectQuery : IQuery<List<MenuResponse>>
    {
        public Guid ProjectId { get; set; }
    }
}
