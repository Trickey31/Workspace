namespace Workspace.Contract
{
    public class GetTaskByJQLQuery : IQuery<List<TaskResponse>>
    {
        public string? Jql { get; set; }
    }
}
