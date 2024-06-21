using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Security.Claims;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using BottomBorder = DocumentFormat.OpenXml.Wordprocessing.BottomBorder;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using LeftBorder = DocumentFormat.OpenXml.Wordprocessing.LeftBorder;
using RightBorder = DocumentFormat.OpenXml.Wordprocessing.RightBorder;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using TopBorder = DocumentFormat.OpenXml.Wordprocessing.TopBorder;

namespace Workspace.Application
{
    public class TaskQueryHandler : IQueryHandler<GetTasksQuery, List<TaskResponse>>,
                                    IQueryHandler<GetTaskByIdQuery, TaskResponse>,
                                    IQueryHandler<GetTaskByUserIdQuery, List<TaskResponse>>,
                                    IQueryHandler<GetTaskByProjectQuery, List<TaskResponse>>,
                                    IQueryHandler<GetPersonalTaskQuery, List<TaskResponse>>,
                                    IQueryHandler<GetTaskStatisticByYearQuery, List<TaskStatisticResponse>>,
                                    IQueryHandler<GetTaskByJQLQuery, List<TaskResponse>>,
                                    IQueryHandler<GetTaskAdvancedQuery, List<TaskResponse>>,
                                    IQueryHandler<GetSuggestionQuery, SuggestionResponse>,
                                    IQueryHandler<GetTaskDueSoonQuery, List<TaskResponse>>,
                                    IQueryHandler<ExportWordQuery, byte[]>,
                                    IQueryHandler<ExportExcelQuery, byte[]>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryBase<Tasks, Guid> _taskRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryBase<Project, Guid> _projectRepository;
        private readonly IRepositoryBase<Users_Projects, Guid> _userProjectRepository;
        private readonly IRepositoryBase<Parent_CTerm, Guid> _parentCTermRepository;
        private readonly IRepositoryBase<CTerm, Guid> _ctermRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskQueryHandler(IMapper mapper, IRepositoryBase<Tasks, Guid> taskRepository, UserManager<User> userManager, IRepositoryBase<Project, Guid> projectRepository, IHttpContextAccessor httpContextAccessor, IRepositoryBase<Users_Projects, Guid> userProjectRepository, IRepositoryBase<Parent_CTerm, Guid> parentCTermRepository, IRepositoryBase<CTerm, Guid> ctermRepository)
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _userManager = userManager;
            _projectRepository = projectRepository;
            _httpContextAccessor = httpContextAccessor;
            _userProjectRepository = userProjectRepository;
            _parentCTermRepository = parentCTermRepository;
            _ctermRepository = ctermRepository;
        }

        public async Task<TResult<List<TaskResponse>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Tasks, bool>> filter = x => x.IsDelete == Constants.IS_DELETE;

            if(request.Status != null)
            {
                filter = filter.And(x => x.Status == request.Status);
            }

            if(request.ProjectId != null)
            {
                filter = filter.And(x => x.ProjectId == request.ProjectId);
            }

            if(request.UserId != null)
            {
                filter = filter.And(x => x.UserId == request.UserId);
            }

            if(!string.IsNullOrEmpty(request.SearchTerm))
            {
                filter = filter.And(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            var task = _taskRepository.FindAll(filter);

            var user = _userManager.Users;

            var query = from a in task
                        join b in user on a.UserId equals b.Id
                        select new TaskResponse
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Type = (Guid)a.Type,
                            Description = a.Description,
                            Priority = a.Priority,
                            Status = a.Status,
                            EndDate = a.EndDate,
                            UserId = b.Id,
                            AssigneeName = b.UserName
                        };

            return query?.ToList();
        }

        public async Task<TResult<TaskResponse>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.FindByIdAsync(request.Id);

            if(entity == null || entity.IsDelete == Constants.DELETED) 
            {
                throw new NotFoundException("Id not found");
            }

            var user = await _userManager.FindByIdAsync(entity.UserId.ToString());

            var userReport = await _userManager.FindByIdAsync(entity.ReporterId.ToString());

            //var parent_cterm = await _parentCTermRepository.FindSingleAsync(x => x.ParentId == request.Id && x.IsDelete == Constants.IS_DELETE);

            //if(parent_cterm == null)
            //{
            //    return null;
            //}

            var response = new TaskResponse();

            if(entity.Type != null || entity.Type != default)
            {
                var cterm = await _ctermRepository.FindByIdAsync((Guid)entity.Type);

                response = new TaskResponse
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    Type = cterm.Id,
                    TypeName = cterm.Name,
                    Priority = entity.Priority,
                    PriorityName = entity.Priority == 1 ? "LOW" : entity.Priority == 2 ? "MEDIUM" : entity.Priority == 3 ? "HIGH" : "",
                    Status = entity.Status,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    UserId = user.Id,
                    AssigneeName = user.Name,
                    ProjectId = entity.ProjectId,
                    ReporterId = userReport.Id,
                    ReporterName = userReport.Name,
                    CreatedDate = entity.CreatedDate,
                    UpdatedDate = entity.UpdatedDate,
                };
            } else
            {
                response = new TaskResponse
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    UserId = user.Id,
                    AssigneeName = user.UserName,
                    ReporterId = userReport.Id,
                    ReporterName = userReport.Name,
                    CreatedDate = entity.CreatedDate,
                    UpdatedDate = entity.UpdatedDate,
                };
            }

            return response;
        }

        public async Task<TResult<List<TaskResponse>>> Handle(GetTaskByUserIdQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if(email == null)
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return null;
            }

            var task = _taskRepository.FindAll(x => x.UserId == user.Id && x.IsDelete == Constants.IS_DELETE && x.ProjectId != null);

            if (task == null)
            {
                return null;
            }

            var userProject = _userProjectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var cterm = _ctermRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var query = from a in task
                        join c in cterm on a.Type equals c.Id
                        join d in userProject on a.UserId equals d.UserId
                        select new TaskResponse
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Description = a.Description,
                            Priority = a.Priority,
                            Type = c.Id,
                            TypeImg = c.CssClass,
                            Status = a.Status,
                            StartDate = a.StartDate,
                            EndDate = a.EndDate,
                            UserId = user.Id,
                            AssigneeName = user.Name,
                            CreatedDate = c.CreatedDate,
                            UpdatedDate = c.UpdatedDate,
                            ProjectId = a.ProjectId,
                        };

            var response = query.Distinct().OrderBy(x => x.CreatedDate).ToList();

            return response;
        }

        public async Task<TResult<List<TaskResponse>>> Handle(GetTaskByProjectQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            Expression<Func<Tasks, bool>> filter = x => x.IsDelete == Constants.IS_DELETE;

            if (request.ProjectId != null)
            {
                filter = filter.And(x => x.ProjectId == request.ProjectId);
            }

            if (request.Status != null)
            {
                filter = filter.And(x => x.Status == request.Status);
            }

            if (request.Priority != null)
            {
                filter = filter.And(x => x.Priority == request.Priority);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                filter = filter.And(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            if(request.CreatedDate != null)
            {
                filter = filter.And(x => x.CreatedDate.Value.Date == request.CreatedDate.Value.Date.AddDays(-1));
            }

            var userProject = _userProjectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.UserId == user.Id);

            var project = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var task = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId != null);

            var users = _userManager.Users;

            var cterm = _ctermRepository.FindAll(x => x.IsDelete == 0);

            //var parent_cterm = _parentCTermRepository.FindAll(x => x.IsDelete == 0);

            var query = from a in userProject
                        join b in project on a.ProjectId equals b.Id
                        join c in task.Where(filter) on b.Id equals c.ProjectId
                        join d in users on c.UserId equals d.Id
                        join e in users on c.ReporterId equals e.Id
                        join g in cterm on c.Type equals g.Id
                        select new TaskResponse
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Priority = c.Priority,
                            Status = c.Status,
                            StartDate = c.StartDate,
                            EndDate = c.EndDate,
                            ProjectName = b.Name,
                            ProjectSlug = b.Slug,
                            UserId = d.Id,
                            AssigneeName = d.Name,
                            ImgAssignee = d.ImgLink,
                            ReporterId = e.Id,
                            ReporterName = e.Name,
                            ImgReporter = e.ImgLink,
                            CreatedDate = c.CreatedDate,
                            UpdatedDate = c.UpdatedDate,
                            Type = g.Id,
                            TypeName = g.Name,
                            TypeImg = g.CssClass,
                        };

            Expression<Func<TaskResponse, bool>> filterFinal = x => true;

            if (request.AssigneeId != null)
            {
                filterFinal = filterFinal.And(x => x.UserId == request.AssigneeId);
            }

            if(request.ReporterId != null)
            {
                filterFinal = filterFinal.And(x => x.ReporterId == request.ReporterId);
            }

            if(request.Type != null)
            {
                filterFinal = filterFinal.And(x => x.Type == request.Type);
            }

            return query.Where(filterFinal).OrderBy(x => x.Id).ToList();
        }

        public async Task<TResult<List<TaskResponse>>> Handle(GetPersonalTaskQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var task = _taskRepository.FindAll(x => x.UserId == user.Id && x.IsDelete == Constants.IS_DELETE && x.ProjectId == null);

            if (task == null)
            {
                return null;
            }

            var response = _mapper.Map<List<TaskResponse>>(task);

            return response;
        }

        public async Task<TResult<List<TaskStatisticResponse>>> Handle(GetTaskStatisticByYearQuery request, CancellationToken cancellationToken)
        {
            var result = new List<TaskStatisticResponse>();

            if(request.Year != null)
            {
                var list = _taskRepository.FindAll(x => x.CreatedDate.Value.Year == request.Year);

                var allMonths = Enumerable.Range(1, 12);

                result = allMonths.GroupJoin(list,
                                                month => month,
                                                item => item.CreatedDate.Value.Month,
                                                (month, item) => new TaskStatisticResponse
                                                {
                                                    Label = "T" + month.ToString() + "/" + request.Year.ToString(),
                                                    Quantity = item.Count()
                                                }).ToList();
            } else
            {
                var totalTasksWithProjectId = _taskRepository.FindAll(x => x.ProjectId != null && x.IsDelete == Constants.IS_DELETE).Count();

                result.Add(new TaskStatisticResponse
                {
                    Label = "Number of Tasks",
                    Quantity = totalTasksWithProjectId
                });
            }

            return result;
        }

        public async Task<TResult<List<TaskResponse>>> Handle(GetTaskByJQLQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var userProject = _userProjectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.UserId == user.Id);

            var project = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var task = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId != null);

            var users = _userManager.Users;

            var cterm = _ctermRepository.FindAll(x => x.IsDelete == 0);

            var query = from a in userProject
                        join b in project on a.ProjectId equals b.Id
                        join c in task on b.Id equals c.ProjectId
                        join d in users on c.UserId equals d.Id
                        join e in users on c.ReporterId equals e.Id
                        join g in cterm on c.Type equals g.Id
                        select new TaskResponse
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Priority = c.Priority,
                            Status = c.Status,
                            StartDate = c.StartDate,
                            EndDate = c.EndDate,
                            ProjectName = b.Name,
                            ProjectSlug = b.Slug,
                            UserId = d.Id,
                            AssigneeName = d.Name,
                            ImgAssignee = d.ImgLink,
                            ReporterId = e.Id,
                            ReporterName = e.Name,
                            ImgReporter = e.ImgLink,
                            CreatedDate = c.CreatedDate,
                            UpdatedDate = c.UpdatedDate,
                            Type = g.Id,
                            TypeName = g.Name,
                            TypeImg = g.CssClass,
                            PriorityName = c != null ? ((int)c.Priority == 1 ? "LOW" : (int)c.Priority == 2 ? "MEDIUM" : (int)c.Priority == 3 ? "HIGH" : "") : "",
                            StatusName = (int)c.Status == 1 ? "To Do" : (int)c.Status == 2 ? "In Progress" : (int)c.Status == 3 ? "Done" : "",
                        };

            if (!string.IsNullOrEmpty(request.Jql))
            {
                query = ApplyJqlFilter(query, request.Jql, email);
            }

            return query.ToList();
        }

        private IQueryable<TaskResponse> ApplyJqlFilter(IQueryable<TaskResponse> query, string jql, string currentUserEmail)
        {
            var conditions = jql.Split(new[] { "AND", "OR" }, StringSplitOptions.None);
            foreach (var condition in conditions)
            {
                if (condition.Contains("project = "))
                {
                    var projectName = ExtractValue(condition, "project = ");
                    query = query.Where(issue => issue.ProjectName.ToLower() == projectName.ToLower());
                }
                else if (condition.Contains("assignee = currentUser()"))
                {
                    query = query.Where(issue => issue.AssigneeName == currentUserEmail);
                }
                else if (condition.Contains("assignee = "))
                {
                    var assignee = ExtractValue(condition, "assignee = ");
                    query = query.Where(issue => issue.AssigneeName.ToLower() == assignee.ToLower());
                }
                else if (condition.Contains("reporter = "))
                {
                    var reporter = ExtractValue(condition, "reporter = ");
                    query = query.Where(issue => issue.ReporterName.ToLower() == reporter.ToLower());
                }
                else if (condition.Contains("priority = "))
                {
                    var priority = ExtractValue(condition, "priority = ");
                    query = query.Where(issue => issue.PriorityName.ToLower() == priority.ToLower());
                }
                else if (condition.Contains("status = "))
                {
                    var status = ExtractValue(condition, "status = ");
                    query = query.Where(issue => issue.StatusName.ToLower() == status.ToLower());
                }
                else if (condition.Contains("type = "))
                {
                    var type = ExtractValue(condition, "type = ");
                    query = query.Where(issue => issue.TypeName.ToLower() == type.ToLower());
                }
                else if (condition.Contains("ORDER BY "))
                {
                    var orderBy = ExtractValue(condition, "ORDER BY ");
                    query = ApplyOrderBy(query, orderBy);
                }
            }

            return query;
        }

        public async Task<TResult<List<TaskResponse>>> Handle(GetTaskAdvancedQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return null;
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var userProject = _userProjectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.UserId == user.Id).ToList();
            var project = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE).ToList();
            var task = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId != null).ToList();

            if(request.ProjectId != null)
            {
                task = task.Where(x => x.ProjectId == request.ProjectId).ToList();
            }

            var users = _userManager.Users.ToList();
            var cterm = _ctermRepository.FindAll(x => x.IsDelete == 0).ToList();

            var query = from a in userProject
                        join b in project on a.ProjectId equals b.Id
                        join c in task on b.Id equals c.ProjectId
                        join d in users on c.UserId equals d.Id
                        join e in users on c.ReporterId equals e.Id
                        join g in cterm on c.Type equals g.Id
                        select new TaskResponse
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Priority = c.Priority,
                            Status = c.Status,
                            StartDate = c.StartDate,
                            EndDate = c.EndDate,
                            ProjectName = b.Name,
                            ProjectSlug = b.Slug,
                            UserId = d.Id,
                            AssigneeName = d.Name,
                            ImgAssignee = d.ImgLink,
                            ReporterId = e.Id,
                            ReporterName = e.Name,
                            ImgReporter = e.ImgLink,
                            CreatedDate = c.CreatedDate,
                            UpdatedDate = c.UpdatedDate,
                            Type = g.Id,
                            TypeName = g.Name,
                            TypeImg = g.CssClass,
                            PriorityName = (int)c.Priority == 1 ? "Low" : (int)c.Priority == 2 ? "Medium" : (int)c.Priority == 3 ? "High" : "",
                            StatusName = (int)c.Status == 1 ? "To Do" : (int)c.Status == 2 ? "In Progress" : (int)c.Status == 3 ? "Done" : "",
                        };

            Expression<Func<TaskResponse, bool>> filter = x => true;

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                filter = filter.And(x => x.ProjectName.ToLower().Contains(request.SearchTerm.ToLower())
                                      || x.ReporterName.ToLower().Contains(request.SearchTerm.ToLower())
                                      || x.AssigneeName.ToLower().Contains(request.SearchTerm.ToLower())
                                      || x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            if (request.Priority != null && !string.IsNullOrEmpty(request.Priority.Value))
            {
                filter = ApplyFilter(filter, d => d.PriorityName, request.Priority);
            }

            if (request.Status != null && !string.IsNullOrEmpty(request.Status.Value))
            {
                filter = ApplyFilter(filter, d => d.StatusName, request.Status);
            }

            if (request.Type != null && !string.IsNullOrEmpty(request.Type.Value))
            {
                filter = ApplyFilter(filter, d => d.TypeName, request.Type);
            }

            if (request.CreatedDate != null && !string.IsNullOrEmpty(request.CreatedDate.Value))
            {
                filter = ApplyDateFilter(filter, d => (DateTime)d.CreatedDate, request.CreatedDate);
            }

            if (request.EndDate != null && !string.IsNullOrEmpty(request.EndDate.Value))
            {
                filter = ApplyDateFilter(filter, d => d.EndDate, request.EndDate);
            }

            if (request.UpdatedDate != null && !string.IsNullOrEmpty(request.UpdatedDate.Value))
            {
                filter = ApplyDateFilter(filter, d => (DateTime)d.UpdatedDate, request.UpdatedDate);
            }

            return query.Where(filter.Compile()).ToList();
        }

        private string ExtractValue(string condition, string key)
        {
            return condition.Split(new[] { key }, StringSplitOptions.None)[1].Trim().Trim('"');
        }

        private IQueryable<TaskResponse> ApplyOrderBy(IQueryable<TaskResponse> query, string orderBy)
        {
            if (orderBy.EndsWith(" ASC"))
            {
                var field = orderBy.Replace(" ASC", "").Trim();
                return query.OrderBy(GetKeySelector(field));
            }
            else if (orderBy.EndsWith(" DESC"))
            {
                var field = orderBy.Replace(" DESC", "").Trim();
                return query.OrderByDescending(GetKeySelector(field));
            }
            else
            {
                return query;
            }
        }

        private Expression<Func<TaskResponse, object>> GetKeySelector(string field)
        {
            switch (field)
            {
                case "name":
                    return task => task.Name;
                case "status":
                    return task => task.Status;
                case "priority":
                    return task => task.Priority;
                case "type":
                    return task => task.TypeName;
                case "assignee":
                    return task => task.AssigneeName;
                case "reporter":
                    return task => task.ReporterName;
                case "project":
                    return task => task.ProjectName;
                case "created":
                    return task => task.CreatedDate;
                default:
                    return task => task.Id;
            }
        }

        private Expression<Func<TaskResponse, bool>> ApplyFilter(Expression<Func<TaskResponse, bool>> data, Func<TaskResponse, string> selector, FilterCriteria criteria)
        {
            switch (criteria.Operator)
            {
                case "=":
                     data = data.And(d => selector(d) == criteria.Value);
                    return data;
                case "!=":
                    data = data.And(d => selector(d) != criteria.Value);
                    return data;
                default:
                    return data;
            }
        }

        private Expression<Func<TaskResponse, bool>> ApplyDateFilter(Expression<Func<TaskResponse, bool>> data, Func<TaskResponse, DateTime> selector, FilterCriteria criteria)
        {
            if (DateTime.TryParse(criteria.Value, out var dateValue))
            {
                var dateOnly = dateValue.Date;
                switch (criteria.Operator)
                {
                    case "=":
                        data = data.And(d => selector(d).Date.AddDays(1) == dateOnly);
                        return data;
                    case "!=":
                        data = data.And(d => selector(d).Date.AddDays(1) != dateOnly);
                        return data;
                    case "<=":
                        data = data.And(d => selector(d).Date.AddDays(1) <= dateOnly);
                        return data;
                    case ">=":
                        data = data.And(d => selector(d).Date.AddDays(1) >= dateOnly);
                        return data;
                    case "<":
                        data = data.And(d => selector(d).Date.AddDays(1) < dateOnly);
                        return data;
                    case ">":
                        data = data.And(d => selector(d).Date.AddDays(1) > dateOnly);
                        return data;
                    default:
                        return data;
                }
            }
            return data;
        }

        public async Task<TResult<SuggestionResponse>> Handle(GetSuggestionQuery request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(request.Query))
            {
                return new SuggestionResponse();
            }

            var lastWord = request.Query.Split(' ').Last();
            var suggestions = new SuggestionResponse
            {
                Keywords = Keywords.Where(k => k.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase)).ToList(),
                Operators = Operators,
                LogicalOperators = LogicalOperators,
            };

            return suggestions;
        }

        public async Task<TResult<List<TaskResponse>>> Handle(GetTaskDueSoonQuery request, CancellationToken cancellationToken)
        {
            var tomorrow = DateTime.UtcNow.AddDays(1);

            var tasks = _taskRepository.FindAll(x => x.EndDate.AddHours(7).Date == tomorrow.Date && x.Status != Constants.DONE).ToList();

            return _mapper.Map<List<TaskResponse>>(tasks);
        }

        public async Task<TResult<byte[]>> Handle(ExportWordQuery request, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.FindByIdAsync(request.TaskId);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                throw new NotFoundException("Id not found");
            }

            var user = await _userManager.FindByIdAsync(entity.UserId.ToString());
            var userReport = await _userManager.FindByIdAsync(entity.ReporterId.ToString());
            var cterm = await _ctermRepository.FindByIdAsync((Guid)entity.Type);
            var project = await _projectRepository.FindByIdAsync((Guid)entity.ProjectId);

            var res = new TaskResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Type = cterm.Id,
                TypeName = cterm.Name,
                Priority = entity.Priority,
                PriorityName = entity.Priority == 1 ? "Low" : entity.Priority == 2 ? "Medium" : entity.Priority == 3 ? "High" : "",
                Status = entity.Status,
                StatusName = entity.Status == 1 ? "To Do" : entity.Status == 2 ? "In Progress" : entity.Status == 3 ? "Done" : "",
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                UserId = user.Id,
                AssigneeName = user.Name,
                ProjectId = entity.ProjectId,
                ProjectName = project.Name,
                ReporterId = userReport.Id,
                ReporterName = userReport.Name,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
            };

            using MemoryStream stream = new();
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                StyleDefinitionsPart stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
                Styles styles = new Styles();

                stylePart.Styles = new Styles();
                stylePart.Styles.Append(new Style()
                {
                    Type = StyleValues.Paragraph,
                    StyleId = "TimesNewRoman",
                    CustomStyle = true,
                    StyleName = new StyleName() { Val = "Times New Roman" },
                    BasedOn = new BasedOn() { Val = "Normal" },
                    Rsid = new Rsid() { Val = "00112233" },
                });
                stylePart.Styles.Save();

                void AddParagraph(string text, bool isBold = false)
                {
                    Paragraph paragraph = new Paragraph();
                    Run run = new Run(new Text(text));

                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new RunFonts() { Ascii = "Times New Roman" });
                    if (isBold)
                    {
                        runProperties.Append(new Bold());
                    }
                    run.RunProperties = runProperties;

                    paragraph.Append(run);
                    body.Append(paragraph);
                }

                void AddTableRow(Table table, string key, string value, bool isHeader = false)
                {
                    TableRow row = new TableRow();

                    TableCell cell1 = new TableCell(new Paragraph(new Run(new Text(key))));
                    TableCellProperties cellProps1 = new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "2400" },
                        new TableCellBorders(
                            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                        )
                    );
                    cell1.Append(cellProps1);

                    TableCell cell2 = new TableCell(new Paragraph(new Run(new Text(value))));
                    TableCellProperties cellProps2 = new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "6600" },
                        new TableCellBorders(
                            new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                        )
                    );
                    cell2.Append(cellProps2);

                    row.Append(cell1, cell2);
                    table.Append(row);
                }

                // Add title
                AddParagraph($"[{project.Name}] {entity.Name}", isBold: true);

                // Create table
                Table table = new Table();
                TableProperties tableProps = new TableProperties(
                    new TableWidth { Width = "9000", Type = TableWidthUnitValues.Dxa }, // Setting table width to fit within A4 page
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                    )
                );

                table.AppendChild(tableProps);

                // Add key-value pairs as table rows
                AddTableRow(table, "Status", res.StatusName);
                AddTableRow(table, "Project", res.ProjectName);
                AddTableRow(table, "Type", res.TypeName);
                AddTableRow(table, "Priority", res.PriorityName);
                AddTableRow(table, "Reporter", res.ReporterName);
                AddTableRow(table, "Assignee", res.AssigneeName);
                AddTableRow(table, "Created date", res.CreatedDate.ToString());
                AddTableRow(table, "Updated date", res.UpdatedDate.ToString());
                AddTableRow(table, "Start date", res.StartDate.ToString());
                AddTableRow(table, "End date", res.EndDate.ToString());

                // Add table to document body
                body.Append(table);

                // Add footer
                AddParagraph($"Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC by Thành Tiến using Workspace.");

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return stream.ToArray();
        }

        public async Task<TResult<byte[]>> Handle(ExportExcelQuery request, CancellationToken cancellationToken)
        {
            var userProject = _userProjectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var project = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var task = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            if(request.ProjectId != null)
            {
                task = task.Where(x => x.ProjectId == request.ProjectId);
            }

            var users = _userManager.Users;

            var cterm = _ctermRepository.FindAll(x => x.IsDelete == 0);

            var query = from a in userProject
                        join b in project on a.ProjectId equals b.Id
                        join c in task on b.Id equals c.ProjectId
                        join d in users on c.UserId equals d.Id
                        join e in users on c.ReporterId equals e.Id
                        join g in cterm on c.Type equals g.Id
                        select new TaskResponse
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Priority = c.Priority,
                            PriorityName = c.Priority == 1 ? "LOW" : c.Priority == 2 ? "MEDIUM" : c.Priority == 3 ? "HIGH" : "",
                            Status = c.Status,
                            StatusName = c.Status == 1 ? "To Do" : c.Status == 2 ? "In Progress" : c.Status == 3 ? "Done" : "",
                            StartDate = c.StartDate,
                            EndDate = c.EndDate,
                            ProjectName = b.Name,
                            ProjectSlug = b.Slug,
                            UserId = d.Id,
                            AssigneeName = d.Name,
                            ImgAssignee = d.ImgLink,
                            ReporterId = e.Id,
                            ReporterName = e.Name,
                            ImgReporter = e.ImgLink,
                            CreatedDate = c.CreatedDate,
                            UpdatedDate = c.UpdatedDate,
                            Type = g.Id,
                            TypeName = g.Name,
                            TypeImg = g.CssClass,
                        };

            var taskList = query.ToList();

            // Generate Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tasks");

                // Add header
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Priority";
                worksheet.Cells[1, 5].Value = "Status";
                worksheet.Cells[1, 6].Value = "Start Date";
                worksheet.Cells[1, 7].Value = "End Date";
                worksheet.Cells[1, 8].Value = "Project Name";
                worksheet.Cells[1, 9].Value = "Project Slug";
                worksheet.Cells[1, 10].Value = "Assignee Name";
                worksheet.Cells[1, 11].Value = "Reporter Name";
                worksheet.Cells[1, 12].Value = "Created Date";
                worksheet.Cells[1, 13].Value = "Updated Date";
                worksheet.Cells[1, 14].Value = "Type Name";

                for (int i = 0; i < taskList.Count; i++)
                {
                    var t = taskList[i];
                    worksheet.Cells[i + 2, 1].Value = t.Id;
                    worksheet.Cells[i + 2, 2].Value = t.Name;
                    worksheet.Cells[i + 2, 3].Value = t.Description;
                    worksheet.Cells[i + 2, 4].Value = t.PriorityName;
                    worksheet.Cells[i + 2, 5].Value = t.StatusName;
                    worksheet.Cells[i + 2, 6].Value = t.StartDate;
                    worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 7].Value = t.EndDate;
                    worksheet.Cells[i + 2, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 8].Value = t.ProjectName;
                    worksheet.Cells[i + 2, 9].Value = t.ProjectSlug;
                    worksheet.Cells[i + 2, 10].Value = t.AssigneeName;
                    worksheet.Cells[i + 2, 11].Value = t.ReporterName;
                    worksheet.Cells[i + 2, 12].Value = t.CreatedDate;
                    worksheet.Cells[i + 2, 12].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 13].Value = t.UpdatedDate;
                    worksheet.Cells[i + 2, 13].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 14].Value = t.TypeName;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return as byte array
                var file = package.GetAsByteArray();
                return TResult<byte[]>.Success(file);
            }
        }

        private static readonly List<string> Keywords = ["project", "status", "assignee", "reporter", "priority"];
        private static readonly List<string> Operators = ["=", "!=", ">", "<", ">=", "<="];
        private static readonly List<string> LogicalOperators = ["AND", "OR"];
    }
}
