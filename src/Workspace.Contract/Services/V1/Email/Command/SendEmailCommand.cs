namespace Workspace.Contract
{
    public class SendEmailCommand : ICommand
    {
        public string ToEmail { get; set; }
    }
}
