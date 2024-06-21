namespace Workspace.Domain
{
    public class Project : EntityAuditBase<Guid>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public int? Status { get; set; }

        public string Slug { get; set; }

        public string ImgLink { get; set; }

        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual ICollection<Users_Projects> Users_Projects { get; set; }
    }
}
