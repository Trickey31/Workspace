namespace Workspace.Contract
{
    public static class TaskDomainEvent
    {
        public record TaskCreated(Guid Id) : IDomainEvent;

        public record TaskUpdated(Guid Id) : IDomainEvent;

        public record TaskDeleted(Guid Id) : IDomainEvent;
    }
}
