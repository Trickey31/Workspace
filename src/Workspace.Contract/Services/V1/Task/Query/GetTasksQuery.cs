namespace Workspace.Contract
{
    public class GetTasksQuery : IQuery<List<TaskResponse>>
    {
        public int? Status { get; set; }

        public Guid? ProjectId { get; set; }

        public Guid? UserId { get; set; }

        public string? SearchTerm { get; set; }
    }
}
