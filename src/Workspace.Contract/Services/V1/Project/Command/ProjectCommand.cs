namespace Workspace.Contract
{
    public class ProjectCommand : ICommand<ProjectCommandResponse>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public int? Status { get; set; }

        public string? ImgLink { get; set; }
    }
}
