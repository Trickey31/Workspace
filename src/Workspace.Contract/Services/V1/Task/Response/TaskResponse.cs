namespace Workspace.Contract
{
    public class TaskResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid Type { get; set; }

        public string TypeName { get; set; }

        public string TypeImg {  get; set; }

        public int? Priority { get; set; }

        public string? PriorityName { get; set; }

        public int? Status { get; set; }

        public string? StatusName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid UserId { get; set; }

        public string AssigneeName { get; set; }

        public string ImgAssignee {  get; set; }

        public Guid ReporterId { get; set; }

        public string ReporterName { get; set; }

        public string ImgReporter {  get; set; }

        public Guid? ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectSlug { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
