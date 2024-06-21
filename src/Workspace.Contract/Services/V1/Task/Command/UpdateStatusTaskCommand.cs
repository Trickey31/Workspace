namespace Workspace.Contract
{
    public class UpdateStatusTaskCommand : ICommand
    {
        public Guid Id { get; set; }

        public int Status { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Description { get; set; }
    }
}
