namespace Workspace.Domain
{
    public interface IDateTracking
    {
        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
