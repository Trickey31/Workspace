using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class LogService : ILogService
    {
        private readonly IRepositoryBase<Logs, Guid> _logRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LogService(IRepositoryBase<Logs, Guid> logRepository, IHttpContextAccessor httpContext, UserManager<User> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _logRepository = logRepository;
            _httpContext = httpContext;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result> CreateLog(string functionType, string functionName, string application, string? beforeValue, string? afterValue, Guid objId)
        {
            var email = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return Result.Failure(new Error("401", "Unauthorized"));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Failure(new Error("401", "Unauthorized"));
            }

            var entity = new Logs
            {
                UserName = user.UserName,
                FullName = user.Name,
                FunctionType = functionType,
                FunctionName = functionName,
                Application = application,
                BeforeValue = beforeValue,
                AfterValue = afterValue,
                ObjId = objId,
            };

            _logRepository.Add(entity);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
