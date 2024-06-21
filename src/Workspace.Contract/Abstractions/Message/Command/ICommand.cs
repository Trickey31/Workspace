using MediatR;

namespace Workspace.Contract
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<TResult<TResponse>>
    {

    }
}
