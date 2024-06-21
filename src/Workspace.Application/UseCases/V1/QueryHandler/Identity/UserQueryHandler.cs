using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class UserQueryHandler : IQueryHandler<GetCurrentUserInfoQuery, CurrentUserInfoResponse>,
                                    IQueryHandler<GetListAssigneeByUserQuery, List<CurrentUserInfoResponse>>,
                                    IQueryHandler<GetUserInProjectQuery, List<UserResponse>>,
                                    IQueryHandler<GetListUserQuery, List<CurrentUserInfoResponse>>,
                                    IQueryHandler<GetUserStatisticByYearQuery, List<UserStatisticByYearResponse>>,
                                    IQueryHandler<GetUserQuery, bool>,
                                    IQueryHandler<GetUserByIdQuery, UserResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRepositoryBase<Users_Projects, Guid> _userProjectRepository;
        private readonly IRepositoryBase<Project, Guid> _projectRepository;
        private readonly IMapper _mapper;

        public UserQueryHandler(UserManager<User> userManager, 
                                IHttpContextAccessor httpContextAccessor,
                                IJwtTokenService jwtTokenService,
                                IRepositoryBase<Users_Projects, Guid> userProjectRepository,
                                IMapper mapper,
                                IRepositoryBase<Project, Guid> projectRepository)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _jwtTokenService = jwtTokenService;
            _userProjectRepository = userProjectRepository;
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<TResult<CurrentUserInfoResponse>> Handle(GetCurrentUserInfoQuery request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers;

            string accessToken = headers["Authorization"];

            if (string.IsNullOrEmpty(accessToken))
            {
                return Result.Failure<CurrentUserInfoResponse>(new Error("401", "Unauthorized"));
            }

            string[] tokenParts = accessToken.Split(' ');
            if (tokenParts.Length != 2 || tokenParts[0] != "Bearer")
            {
                return Result.Failure<CurrentUserInfoResponse>(new Error("400", "Invalid Access Token"));
            }

            string token = tokenParts[1];

            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token);

            if (principal == null)
            {
                return Result.Failure<CurrentUserInfoResponse>(new Error("401", "Invalid or expired Access Token"));
            }

            string userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(userEmail);

            var role = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                return Result.Failure<CurrentUserInfoResponse>(new Error("404", "User not found"));
            }

            // Tạo đối tượng CurrentUserInfoResponse từ thông tin người dùng
            var userInfo = new CurrentUserInfoResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Name = user.Name,
                ImgLink = user.ImgLink,
                PhoneNumber = user.PhoneNumber,
                Roles = role,
            };

            return Result.Success<CurrentUserInfoResponse>(userInfo);
        }

        public async Task<TResult<List<CurrentUserInfoResponse>>> Handle(GetListAssigneeByUserQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(email);

            var user_project = await _userProjectRepository.FindSingleAsync(x => x.UserId == user.Id && x.ProjectId == request.ProjectId && x.IsDelete == Constants.IS_DELETE);

            if(user_project == null)
            {
                throw new NotFoundException("Not Found");
            }

            var response = new List<CurrentUserInfoResponse>();

            if (user_project.RoleId == 1)
            {
                var users = _userManager.Users
                    .Where(u => u.Users_Projects.Any(up => up.ProjectId == request.ProjectId))
                    .ToList();

                response = users.Select(u => new CurrentUserInfoResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToList();
            }
            else
            {
                response.Add(new CurrentUserInfoResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Email = user.Email
                });
            }

            return response;
        }

        public async Task<TResult<List<UserResponse>>> Handle(GetUserInProjectQuery request, CancellationToken cancellationToken)
        {
            var userProject = _userProjectRepository.FindAll(x => x.ProjectId == request.ProjectId && x.IsDelete == Constants.IS_DELETE);

            if(userProject == null)
            {
                throw new NotFoundException("Records not found");
            }

            var user = _userManager.Users;

            if (user == null)
            {
                throw new NotFoundException("Records not found");
            }

            var query = from a in userProject
                        join b in user on a.UserId equals b.Id
                        select new UserResponse
                        {
                            Id = b.Id,
                            Name = b.Name,
                            Email = b.Email,
                            RoleId = a.RoleId,
                            ImgLink = b.ImgLink,
                        };

            return query.ToList();
        }

        public async Task<TResult<List<CurrentUserInfoResponse>>> Handle(GetListUserQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<User, bool>> filter = x => true;

            if(!string.IsNullOrEmpty(request.KeyWord))
            {
                filter = filter.And(x => x.Name.ToLower().Contains(request.KeyWord.ToLower()) || x.UserName.ToLower().Contains(request.KeyWord.ToLower()) || x.Email.ToLower().Contains(request.KeyWord.ToLower()) || x.PhoneNumber.ToLower().Contains(request.KeyWord.ToLower()));
            }

            var users = await _userManager.Users.Where(filter).OrderByDescending(x => x.Id).ToListAsync();

            var usersWithRoles = new List<CurrentUserInfoResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userInfo = new CurrentUserInfoResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ImgLink = user.ImgLink,
                    Roles = roles.ToList()
                };

                usersWithRoles.Add(userInfo);
            }

            return usersWithRoles;
        }

        public async Task<TResult<List<UserStatisticByYearResponse>>> Handle(GetUserStatisticByYearQuery request, CancellationToken cancellationToken)
        {
            var result = new List<UserStatisticByYearResponse>();

            if(request.Year != null)
            {
                var list = _userManager.Users.Where(x => x.CreatedDate.Year == request.Year);

                var allMonths = Enumerable.Range(1, 12);

                result = allMonths.GroupJoin(list,
                                                month => month,
                                                item => item.CreatedDate.Month,
                                                (month, item) => new UserStatisticByYearResponse
                                                {
                                                    Label = "T" + month.ToString() + "/" + request.Year.ToString(),
                                                    Quantity = item.Count()
                                                }).ToList();
            } else
            {
                var totalTasksWithProjectId = _userManager.Users.Count();

                result.Add(new UserStatisticByYearResponse
                {
                    Label = "Number of Users",
                    Quantity = totalTasksWithProjectId
                });
            }

            return result;
        }

        public async Task<TResult<bool>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return false;
            }

            return true;
        }

        public async Task<TResult<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if(user == null)
            {
                return null;
            }

            return _mapper.Map<UserResponse>(user);
        }
    }
}
