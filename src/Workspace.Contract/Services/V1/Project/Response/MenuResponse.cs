namespace Workspace.Contract
{
    public class MenuResponse
    {
        public Guid Id { get; set; }

        public string Slug { get; set; }

        public string Name { get; set; }

        public string CssClass { get; set; }

        public int Status { get; set; }
    }
}
