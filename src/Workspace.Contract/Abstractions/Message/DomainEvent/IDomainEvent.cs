using MediatR;

namespace Workspace.Contract
{
    public interface IDomainEvent : INotification
    {
        public Guid Id { get; init; }
    }
}
