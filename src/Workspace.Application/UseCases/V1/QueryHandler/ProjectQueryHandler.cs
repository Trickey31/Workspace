using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Security.Claims;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class ProjectQueryHandler : IQueryHandler<GetProjectByIdQuery, ProjectResponse>,
                                       IQueryHandler<GetProjectBySlugQuery, ProjectResponse>,
                                       IQueryHandler<GetListProjectByUserIdQuery, List<ProjectResponse>>,
                                       IQueryHandler<GetStatisticByPriorityQuery, List<StatisticResponse>>,
                                       IQueryHandler<GetStatisticByAssigneeQuery, List<StatisticResponse>>,
                                       IQueryHandler<GetStatisticByStatusQuery, List<StatisticResponse>>,
                                       IQueryHandler<GetStatisticByTaskTypeQuery, List<StatisticResponse>>,
                                       IQueryHandler<GetProjectsQuery, List<ProjectResponse>>,
                                       IQueryHandler<GetStatisticByYearQuery, List<StatisticResponseByYear>>,
                                       IQueryHandler<GetMenuInProjectQuery, List<MenuResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryBase<Project, Guid> _projectRepository;
        private readonly IRepositoryBase<Users_Projects, Guid> _userProjectRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryBase<Tasks, Guid> _taskRepository;
        private readonly IRepositoryBase<CTerm, Guid> _ctermRepository;
        private readonly IRepositoryBase<Parent_CTerm, Guid> _parentCtermRepository;

        public ProjectQueryHandler(IMapper mapper, IRepositoryBase<Project, Guid> projectRepository, IRepositoryBase<Users_Projects, Guid> userProjectRepository, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IRepositoryBase<Tasks, Guid> taskRepository, IRepositoryBase<CTerm, Guid> ctermRepository, IRepositoryBase<Parent_CTerm, Guid> parentCtermRepository)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
            _userProjectRepository = userProjectRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _taskRepository = taskRepository;
            _ctermRepository = ctermRepository;
            _parentCtermRepository = parentCtermRepository;
        }

        public async Task<TResult<ProjectResponse>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _projectRepository.FindByIdAsync(request.Id, cancellationToken);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                throw new NotFoundException("Bản ghi không tồn tại");
            }

            var userProject = await _userProjectRepository.FindSingleAsync(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId == entity.Id && x.RoleId == Constants.ADMINISTRATOR);

            if(userProject == null)
            {
                throw new NotFoundException("Bản ghi không tồn tại");
            }

            var response = new ProjectResponse
            {
                Id = entity.Id,
                Description = entity.Description,
                Name = entity.Name,
                Slug = entity.Slug,
                LeaderId = userProject.UserId,
                ImgLink = entity.ImgLink,
            };

            return response;
        }

        public async Task<TResult<ProjectResponse>> Handle(GetProjectBySlugQuery request, CancellationToken cancellationToken)
        {
            var entity = await _projectRepository.FindSingleAsync(x => x.Slug == request.Slug && x.IsDelete == Constants.IS_DELETE);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                throw new NotFoundException("Bản ghi không tồn tại");
            }

            var user = await _userProjectRepository.FindSingleAsync(x => x.IsDelete == Constants.IS_DELETE && x.RoleId == 1 && x.ProjectId == entity.Id);

            var response = new ProjectResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                Description = entity.Description,
                LeaderId = user.UserId,
                ImgLink = entity.ImgLink,
            };

            return response;
        }

        public async Task<TResult<List<ProjectResponse>>> Handle(GetListProjectByUserIdQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(email);

            var projectId = _userProjectRepository.FindAll(x => x.UserId == user.Id && x.IsDelete == Constants.IS_DELETE).Select(x => x.ProjectId);

            Expression<Func<Project, bool>> filter = x => x.IsDelete == Constants.IS_DELETE && projectId.Contains(x.Id);


            var query = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? _projectRepository.FindAll(x => projectId.Contains(x.Id) && x.IsDelete == Constants.IS_DELETE)
                : _projectRepository.FindAll(x => projectId.Contains(x.Id) && x.IsDelete == Constants.IS_DELETE && (x.Name.Contains(request.SearchTerm) || x.Description.Contains(request.SearchTerm)));

            var result = _mapper.Map<List<ProjectResponse>>(query);
            return Result.Success(result);
        }

        public async Task<TResult<List<StatisticResponse>>> Handle(GetStatisticByPriorityQuery request, CancellationToken cancellationToken)
        {
            var list = new List<StatisticResponse>();

            var task = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId == request.ProjectId && x.Status != Constants.DONE);

            var high = task.Where(x => x.Priority == Constants.HIGH).Count();

            var highTask = new StatisticResponse
            {
                Name = "High",
                Task = high,
                Percentage = (high*100) / task.Count(),
            };

            var medium = task.Where(x => x.Priority == Constants.MEDIUM).Count();

            var mediumTask = new StatisticResponse
            {
                Name = "Medium",
                Task = medium,
                Percentage = (medium*100) / task.Count(),
            };

            var low = task.Where(x => x.Priority == Constants.LOW).Count();

            var lowTask = new StatisticResponse
            {
                Name = "Low",
                Task = low,
                Percentage = (low * 100) / task.Count(),
            };

            list.Add(highTask);
            list.Add(mediumTask);
            list.Add(lowTask);

            return list;
        }

        public async Task<TResult<List<StatisticResponse>>> Handle(GetStatisticByAssigneeQuery request, CancellationToken cancellationToken)
        {
            var tasks = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId == request.ProjectId);

            var users = _userManager.Users;

            if (tasks == null || !tasks.Any())
            {
                return new List<StatisticResponse>();
            }

            var taskGroups = from t in tasks
                             join u in users on t.UserId equals u.Id
                             group t by new { u.Name, u.Id } into g
                             select new
                             {
                                 AssigneeId = g.Key.Id,
                                 AssigneeName = g.Key.Name,
                                 TotalTasks = g.Count(),
                                 UnfinishedTasks = g.Count(t => t.Status != Constants.DONE)
                             };

            var response = taskGroups.Select(g => new StatisticResponse
            {
                Name = g.AssigneeName,
                Task = g.UnfinishedTasks,
                Percentage = g.TotalTasks == 0 ? 0 : (g.UnfinishedTasks * 100) / g.TotalTasks
            }).ToList();

            return response;
        }


        public async Task<TResult<List<StatisticResponse>>> Handle(GetStatisticByStatusQuery request, CancellationToken cancellationToken)
        {
            var task = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId == request.ProjectId);

            var query = from t in task
                        group t by t.Status into g
                        select new StatisticResponse
                        {
                            Name = g.Key.ToString(),
                            Task = g.Count(),
                            Percentage = (g.Count() * 100) / task.Count(),
                        };

            var response = query.ToList();

            return response;
        }

        public async Task<TResult<List<StatisticResponse>>> Handle(GetStatisticByTaskTypeQuery request, CancellationToken cancellationToken)
        {
            var cterm = _ctermRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var tasks = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId == request.ProjectId);

            if (tasks == null || !tasks.Any())
            {
                return new List<StatisticResponse>();
            }

            var taskGroups = from t in tasks
                             join c in cterm on t.Type equals c.Id
                             group t by new { t.Type, c.Name } into g
                             select new
                             {
                                 g.Key.Type,
                                 g.Key.Name,
                                 TotalTasks = g.Count(),
                                 UnfinishedTasks = g.Count(t => t.Status != 3)
                             };

            var response = taskGroups.Select(g => new StatisticResponse
            {
                Name = g.Type.ToString(),
                Task = g.UnfinishedTasks,
                Percentage = g.TotalTasks == 0 ? 0 : (g.UnfinishedTasks * 100) / g.TotalTasks
            }).ToList();

            return response;
        }

        public async Task<TResult<List<ProjectResponse>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var user = _userManager.Users;

            var userProject = _userProjectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.RoleId == Constants.ADMINISTRATOR);

            var query = from a in projects
                        join b in userProject on a.Id equals b.ProjectId
                        join c in user on b.UserId equals c.Id
                        select new ProjectResponse
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Description = a.Description,
                            Slug = a.Slug,
                            ImgLink = a.ImgLink,
                            LeaderId = c.Id,
                            LeaderName = c.UserName,
                            CreatedDate = (DateTime)a.CreatedDate,
                        };

            Expression<Func<ProjectResponse, bool>> filter = x => true;

            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                filter = filter.And(x => x.LeaderName.ToLower().Contains(request.KeyWord.ToLower()) || x.Name.ToLower().Contains(request.KeyWord.ToLower()) || x.Description.ToLower().Contains(request.KeyWord.ToLower()) || x.Slug.ToLower().Contains(request.KeyWord.ToLower()));
            }

            return query.Where(filter).ToList();
        }

        public async Task<TResult<List<StatisticResponseByYear>>> Handle(GetStatisticByYearQuery request, CancellationToken cancellationToken)
        {
            var result = new List<StatisticResponseByYear>();

            if(request.Year != null)
            {
                var list = _projectRepository.FindAll(x => x.CreatedDate.Value.Year == request.Year);

                var allMonths = Enumerable.Range(1, 12);

                result = allMonths.GroupJoin(list,
                                                month => month,
                                                item => item.CreatedDate.Value.Month,
                                                (month, item) => new StatisticResponseByYear
                                                {
                                                    Label = "T" + month.ToString() + "/" + request.Year.ToString(),
                                                    Quantity = item.Count()
                                                }).ToList();
            } else
            {
                var totalTasksWithProjectId = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE).Count();

                result.Add(new StatisticResponseByYear
                {
                    Label = "Number of Projects",
                    Quantity = totalTasksWithProjectId
                });
            }

            return result;
        }

        public async Task<TResult<List<MenuResponse>>> Handle(GetMenuInProjectQuery request, CancellationToken cancellationToken)
        {
            var c_term = _ctermRepository.FindAll(x => x.Type == "C_MENU" && x.IsDelete == Constants.IS_DELETE);

            var menu_cterm = _parentCtermRepository.FindAll(x => x.ParentId == request.ProjectId && x.IsDelete == Constants.IS_DELETE);

            var result = (from a in c_term
                         join b in menu_cterm on a.Id equals b.TypeId
                         select new MenuResponse
                         {
                             Id = b.Id,
                             Name = a.Name,
                             Slug = ToKebabCase(a.Name),
                             CssClass = a.CssClass,
                             Status = int.Parse(a.Description),
                         }).ToList();

            return result;
        }

        private static string ToKebabCase(string input)
        {
            // Tách chuỗi thành các từ
            string[] words = input.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            // Chuyển các từ về chữ thường và nối lại bằng dấu "-"
            return string.Join("-", words.Select(w => w.ToLower()));
        }
    }
}
