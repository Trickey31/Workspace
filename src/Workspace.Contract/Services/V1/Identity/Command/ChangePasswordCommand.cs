namespace Workspace.Contract
{
    public class ChangePasswordCommand : ICommand
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
