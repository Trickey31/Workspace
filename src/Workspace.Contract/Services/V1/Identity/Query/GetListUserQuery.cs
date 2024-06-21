namespace Workspace.Contract
{
    public class GetListUserQuery : IQuery<List<CurrentUserInfoResponse>>
    {
        public string? KeyWord { get; set; }
    }
}
