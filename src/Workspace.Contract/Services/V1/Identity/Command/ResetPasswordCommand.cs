namespace Workspace.Contract
{
    public class ResetPasswordCommand : ICommand
    {
        public string Email { get; set; }
    }
}
