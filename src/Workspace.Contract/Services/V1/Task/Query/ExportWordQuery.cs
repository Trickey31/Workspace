namespace Workspace.Contract
{
    public class ExportWordQuery : IQuery<byte[]>
    {
        public Guid TaskId { get; set; }
    }
}
