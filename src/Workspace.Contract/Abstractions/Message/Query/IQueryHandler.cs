using MediatR;

namespace Workspace.Contract
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResult<TResponse>>
        where TQuery : IQuery<TResponse>
    {
    }
}
