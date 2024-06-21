namespace Workspace.Domain
{
    public class Notification : DomainEntity<Guid>
    {
        public Guid FromUser { get; set; }

        public Guid ToUser { get; set; }

        public string FunctionType { get; set; }

        public string FunctionName { get; set; }

        public Guid ObjId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Type { get; set; }

        public bool HaveSeen { get; set; }

        public bool IsNew { get; set; }
    }
}
