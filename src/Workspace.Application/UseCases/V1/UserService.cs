using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Workspace.Contract;
using Workspace.Domain;

namespace Workspace.Application
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<TResult<User>> GetCurrentUserAsync()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if(email == null)
            {
                return Result.Failure<User>(new Error("401", "Unauthourized"));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Result.Failure<User>(new Error("400", "Email invalid"));
            }

            return user;
        }
    }
}
