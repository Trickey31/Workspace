namespace Workspace.Contract
{
    public class GetLogsQuery : IQuery<List<LogResponse>>
    {
        public string? FunctionType { get; set; }

        public string? KeyWord { get; set; }

        public string? Application {  get; set; }
    }
}
