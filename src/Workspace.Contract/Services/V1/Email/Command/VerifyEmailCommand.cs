namespace Workspace.Contract
{
    public class VerifyEmailCommand : ICommand
    {
        public string Email { get; set; }

        public string Otp { get; set; }
    }
}
