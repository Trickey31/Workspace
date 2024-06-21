using MediatR;
using Workspace.Contract;

namespace Workspace.Application
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResult<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}
