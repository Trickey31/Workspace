using MediatR;

namespace Workspace.Contract
{
    public interface IQuery<TResponse> : IRequest<TResult<TResponse>>
    {
    }
}
