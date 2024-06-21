namespace Workspace.Contract
{
    public class GetUserByIdQuery : IQuery<UserResponse>
    {
        public Guid Id { get; set; }
    }
}
