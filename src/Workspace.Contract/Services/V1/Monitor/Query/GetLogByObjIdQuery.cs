namespace Workspace.Contract
{
    public class GetLogByObjIdQuery : IQuery<List<LogResponse>>
    {
        public Guid ObjId { get; set; }
    }
}
