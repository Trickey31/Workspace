namespace Workspace.Domain
{
    public class CTerm : EntityAuditBase<Guid>
    {
        public string CssClass { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Parent_CTerm> Project_CTerms { get; set; }

    }
}
