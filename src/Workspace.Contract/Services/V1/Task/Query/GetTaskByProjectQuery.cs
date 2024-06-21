namespace Workspace.Contract
{
    public class GetTaskByProjectQuery : IQuery<List<TaskResponse>>
    {
        public Guid? ProjectId { get; set; }

        public int? Status { get; set; }

        public int? Priority { get; set; }

        public string? SearchTerm { get; set; }

        public Guid? AssigneeId { get; set; }

        public Guid? ReporterId { get; set; }

        public Guid? Type { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
