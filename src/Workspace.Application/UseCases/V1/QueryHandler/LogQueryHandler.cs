using AutoMapper;
using LinqKit;
using System.Linq;
using System.Linq.Expressions;
using Workspace.Contract;
using Workspace.Domain;

namespace Workspace.Application
{
    public class LogQueryHandler : IQueryHandler<GetLogByObjIdQuery, List<LogResponse>>,
                                   IQueryHandler<GetLogsQuery, List<LogResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryBase<Logs, Guid> _logRepository;

        public LogQueryHandler(IMapper mapper, IRepositoryBase<Logs, Guid> logRepository)
        {
            _mapper = mapper;
            _logRepository = logRepository;
        }

        public async Task<TResult<List<LogResponse>>> Handle(GetLogByObjIdQuery request, CancellationToken cancellationToken)
        {
            var query = _logRepository.FindAll(x => x.ObjId == request.ObjId).ToList();

            var response = _mapper.Map<List<LogResponse>>(query);

            return response;
        }

        public async Task<TResult<List<LogResponse>>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Logs, bool>> filter = x => true;

            if (!string.IsNullOrEmpty(request.FunctionType))
            {
                filter = filter.And(x => x.FunctionType.ToLower() ==  request.FunctionType.ToLower());
            }

            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                filter = filter.And(x => x.UserName.ToLower().Contains(request.KeyWord.ToLower()) || x.FullName.ToLower().Contains(request.KeyWord.ToLower()) || x.FunctionName.ToLower().Contains(request.KeyWord.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.Application))
            {
                filter = filter.And(x => x.Application.ToLower() == request.Application.ToLower());
            }

            var query = _logRepository.FindAll(filter).OrderByDescending(x => x.CreatedDate).ToList();


            var response = _mapper.Map<List<LogResponse>>(query);


            return response;
        }
    }
}
