namespace Workspace.Contract
{
    public class GetUserQuery : IQuery<bool>
    {
        public string Email { get; set; }
    }
}
