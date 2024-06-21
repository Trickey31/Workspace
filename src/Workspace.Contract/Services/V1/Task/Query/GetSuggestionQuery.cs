namespace Workspace.Contract
{
    public class GetSuggestionQuery : IQuery<SuggestionResponse>
    {
        public string? Query { get; set; }
    }
}
