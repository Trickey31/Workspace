namespace Workspace.Contract
{
    public class NotificationResponse 
    {
        public Guid Id { get; set; }

        public Guid FromUser { get; set; }

        public string FromUserName { get; set; }

        public Guid ToUser { get; set; }

        public int Type { get; set; }

        public bool HaveSeen { get; set; }

        public string FunctionType { get; set; }

        public string FunctionName { get; set; }

        public Guid ObjId { get; set; }

        public string ObjName { get; set; }

        public string Icon { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ParentObjName { get; set; }

        public string? ParentObjSlug { get; set; }

        public bool IsNew { get; set; }
    }
}
