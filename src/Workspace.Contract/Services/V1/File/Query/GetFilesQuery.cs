namespace Workspace.Contract
{
    public class GetFilesQuery : IQuery<List<FileQueryResponse>>
    {
        public Guid? ObjId { get; set; }

        public int ObjKey { get; set; }
    }
}
