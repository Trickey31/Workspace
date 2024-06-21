namespace Workspace.Contract
{
    public class ExportExcelQuery : IQuery<byte[]>
    {
        public Guid? ProjectId { get; set; }
    }
}
