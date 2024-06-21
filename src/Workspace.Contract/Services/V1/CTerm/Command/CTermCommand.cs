namespace Workspace.Contract
{
    public class CTermCommand : ICommand
    {
        public Guid ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CssClass { get; set; }

        public string Type { get; set; }
    }
}
