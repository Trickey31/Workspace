using Workspace.Application;

namespace Workspace.Contract
{
    public static class AuthenCommand
    {
        public record Login(string Email, string Password) : ICommand<AuthResponse.Authenticated>;

        public record Token(string? AccessToken, string? RefreshToken) : ICommand<AuthResponse.Authenticated>;
    }
}
