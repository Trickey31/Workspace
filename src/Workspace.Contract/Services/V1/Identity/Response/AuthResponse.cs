namespace Workspace.Contract
{
    public static class AuthResponse
    {
        public class Authenticated
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
            public DateTime RefreshTokenExpiryTime { get; set; }
        }
    }
}
