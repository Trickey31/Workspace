namespace Workspace.Contract
{
    public class GetUserInProjectQuery : IQuery<List<UserResponse>>
    {
        public Guid ProjectId { get; set; }
    }
}
