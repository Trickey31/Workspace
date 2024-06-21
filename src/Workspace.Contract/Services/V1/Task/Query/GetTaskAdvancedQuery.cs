namespace Workspace.Contract
{
    public class GetTaskAdvancedQuery : IQuery<List<TaskResponse>>
    {
        public string? SearchTerm { get; set; }

        public Guid? ProjectId { get; set; }

        public FilterCriteria? Type { get; set; }

        public FilterCriteria? Priority { get; set; }

        public FilterCriteria? Status { get; set; }

        public FilterCriteria? CreatedDate { get; set; }

        public FilterCriteria? UpdatedDate { get; set; }

        public FilterCriteria? EndDate { get; set; }
    }

    public class FilterCriteria
    {
        public string Operator { get; set; }

        public string? Value { get; set; }
    }
}
