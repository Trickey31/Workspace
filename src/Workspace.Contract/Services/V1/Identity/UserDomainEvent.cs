namespace Workspace.Contract
{
    public static class UserDomainEvent
    {
        public record UserRegisted(Guid Id) : IDomainEvent;
    }
}
