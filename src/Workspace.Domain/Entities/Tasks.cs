namespace Workspace.Domain
{
    public class Tasks : EntityAuditBase<Guid>
    {
        public Guid? ProjectId { get; set; }

        public Guid UserId { get; set; }

        public Guid ReporterId { get; set; }

        public Guid? Type {  get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Priority { get; set; }

        public int? Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
