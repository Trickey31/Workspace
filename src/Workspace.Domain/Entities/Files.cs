namespace Workspace.Domain
{
    public class Files : EntityAuditBase<Guid>
    {
        public string Name { get; set; }

        public int ObjKey { get; set; }

        public Guid? ObjId { get; set; }

        public string Link { get; set; }
    }
}
