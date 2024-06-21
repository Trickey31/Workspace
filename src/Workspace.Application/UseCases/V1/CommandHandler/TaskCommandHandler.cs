using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class TaskCommandHandler : ICommandHandler<CreateTaskCommand>,
                                      ICommandHandler<UpdateTaskCommand>,
                                      ICommandHandler<DeleteTaskCommand>,
                                      ICommandHandler<UpdateStatusTaskCommand>,
                                      ICommandHandler<CreatePersonalTaskCommand>,
                                      ICommandHandler<UpdatePersonalTaskCommand>
    {
        private readonly IRepositoryBase<Tasks, Guid> _taskRepository;
        private readonly IRepositoryBase<Parent_CTerm, Guid> _parentCTermRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ILogService _logService;
        private readonly IRepositoryBase<Notification, Guid> _notificationRepository;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IRepositoryBase<Users_Projects, Guid> _usersProjectsRepository;
        private readonly IEmailService _emailService;

        public TaskCommandHandler(IRepositoryBase<Tasks, Guid> taskRepository,
                                  ApplicationDbContext context,
                                  IMapper mapper,
                                  IHttpContextAccessor httpContextAccessor,
                                  UserManager<User> userManager,
                                  IRepositoryBase<Parent_CTerm, Guid> parentCTermRepository,
                                  ILogService logService,
                                  IRepositoryBase<Notification, Guid> notificationRepository,
                                  IHubContext<NotificationHub> notificationHubContext,
                                  IRepositoryBase<Users_Projects, Guid> userProjectsRepository,
                                  IEmailService emailService)
        {
            _taskRepository = taskRepository;
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _parentCTermRepository = parentCTermRepository;
            _logService = logService;
            _notificationRepository = notificationRepository;
            _notificationHubContext = notificationHubContext;
            _usersProjectsRepository = userProjectsRepository;
            _emailService = emailService;
        }

        public async Task<Result> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            if(request.EndDate != null && request.EndDate.Value.AddDays(1).Date < DateTime.Now.Date)
            {
                return Result.Failure(new Error("400", "EndDate cannot be earlier than the current date"));
            }

            var userProject = await _usersProjectsRepository.FindSingleAsync(x => x.ProjectId == request.ProjectId && x.UserId == request.ReporterId && x.IsDelete == Constants.IS_DELETE);

            if(userProject.RoleId == Constants.VIEWER)
            {
                return Result.Failure(new Error("401", "You don't have permission to create task"));
            }
            
            var entity = _mapper.Map<Tasks>(request);

            entity.Id = Guid.NewGuid();
            entity.StartDate = DateTime.Now.ToUniversalTime().Date.AddHours(-7);
            entity.CreatedDate = DateTime.Now.ToUniversalTime().Date.AddHours(-7);
            entity.IsDelete = 0;

            _taskRepository.Add(entity);
            await _context.SaveChangesAsync();

            await _logService.CreateLog("POST", "Add new task", "TASK", null, JsonConvert.SerializeObject(entity), entity.Id);

            if (request.ReporterId != request.UserId)
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    FromUser = request.ReporterId,
                    ToUser = request.UserId,
                    FunctionType = "POST",
                    FunctionName = "Assigned task",
                    ObjId = entity.Id,
                    Type = Constants.TASK_NOTIFICATION,
                    IsNew = true,
                };

                _notificationRepository.Add(notification);
                await _context.SaveChangesAsync();
                await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", request.UserId.ToString());
                await _emailService.SendEmailV2Async(user.Email, entity.Id, "created");
            }

            return Result.Success();
        }

        public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var list = new List<Guid>();

            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return Result.Failure(new Error("401", "Unauthorize"));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Failure(new Error("404", "Not found"));
            }

            var userProject = await _usersProjectsRepository.FindSingleAsync(x => x.ProjectId == request.ProjectId && x.UserId == user.Id && x.IsDelete == Constants.IS_DELETE);

            if(user.Id != request.ReporterId && userProject.RoleId != Constants.ADMINISTRATOR)
            {
                return Result.Failure(new Error("401", "You don't have permission to update task"));
            }

            if (userProject.RoleId == Constants.VIEWER)
            {
                return Result.Failure(new Error("401", "You don't have permission to update task"));
            }

            var entity = await _taskRepository.FindByIdAsync(request.Id);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", $"Id {request.Id} not found"));
            }

            if (request.EndDate != null && request.EndDate < entity.CreatedDate)
            {
                return Result.Failure(new Error("400", "EndDate cannot be earlier than the current date"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            var newEntity = _mapper.Map(request, entity);

            newEntity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _taskRepository.Update(newEntity);
            await _context.SaveChangesAsync();

            if (user.Id != request.ReporterId && userProject.RoleId == Constants.ADMINISTRATOR)
            {
                var assignee = await _userManager.FindByIdAsync(request.UserId.ToString());
                var reporter = await _userManager.FindByIdAsync(request.ReporterId.ToString());

                list.Add(request.UserId);
                list.Add(request.ReporterId);

                var listNoti = new List<Notification>();

                var notification1 = new Notification
                {
                    Id = Guid.NewGuid(),
                    FromUser = user.Id,
                    ToUser = request.UserId,
                    FunctionType = "PUT",
                    FunctionName = "Updated a task",
                    ObjId = newEntity.Id,
                    Type = Constants.COMMENT_NOTIFICATION,
                    IsNew = true,
                };

                var notification2 = new Notification
                {
                    Id = Guid.NewGuid(),
                    FromUser = user.Id,
                    ToUser = request.ReporterId,
                    FunctionType = "PUT",
                    FunctionName = "Updated a task",
                    ObjId = newEntity.Id,
                    Type = Constants.COMMENT_NOTIFICATION,
                    IsNew = true,
                };

                _notificationRepository.Add(notification1);
                _notificationRepository.Add(notification2);
                await _context.SaveChangesAsync();
                await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", JsonConvert.SerializeObject(list));
                await _emailService.SendEmailV2Async(assignee.Email, newEntity.Id, "updated");
                await _emailService.SendEmailV2Async(reporter.Email, newEntity.Id, "updated");

            }
            else if(user.Id == request.ReporterId && userProject.RoleId == Constants.ADMINISTRATOR)
            {
                var assignee = await _userManager.FindByIdAsync(request.UserId.ToString());

                list.Add(request.UserId);

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    FromUser = user.Id,
                    ToUser = request.UserId,
                    FunctionType = "PUT",
                    FunctionName = "Updated a task",
                    ObjId = newEntity.Id,
                    Type = Constants.COMMENT_NOTIFICATION,
                    IsNew = true,
                };

                _notificationRepository.Add(notification);
                await _context.SaveChangesAsync();
                await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", JsonConvert.SerializeObject(list));
                await _emailService.SendEmailV2Async(assignee.Email, newEntity.Id, "updated");
            }

            await _logService.CreateLog("PUT", "Update task", "TASK", oldEntity, JsonConvert.SerializeObject(newEntity), entity.Id);

            return Result.Success();
        }

        public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.FindByIdAsync(request.Id);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", $"Id {request.Id} found"));
            }

            var userProject = await _usersProjectsRepository.FindSingleAsync(x => x.ProjectId == entity.ProjectId && x.UserId == entity.ReporterId && x.IsDelete == Constants.IS_DELETE);

            if (userProject.RoleId == Constants.VIEWER)
            {
                return Result.Failure(new Error("401", "You don't have permission to delete task"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            entity.IsDelete = Constants.DELETED;
            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _taskRepository.Update(entity);

            await _logService.CreateLog("DELETE", "Delete task", "TASK", oldEntity, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success();
        }

        public async Task<Result> Handle(UpdateStatusTaskCommand request, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.FindByIdAsync(request.Id);

            if(entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", $"Id {request.Id} not found"));
            }

            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return Result.Failure(new Error("401", "Unauthorize"));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Failure(new Error("404", "Not found"));
            }

            var userProject = await _usersProjectsRepository.FindSingleAsync(x => x.ProjectId == entity.ProjectId && x.UserId == user.Id && x.IsDelete == Constants.IS_DELETE);

            if (userProject.RoleId == Constants.VIEWER)
            {
                return Result.Failure(new Error("401", "You don't have permission to update task"));
            }

            if (request.EndDate != null && request.EndDate != entity.EndDate)
            {
                if (request.EndDate.Value.Date.AddDays(1) < entity.CreatedDate.Value.Date)
                {
                    return Result.Failure(new Error("400", "EndDate cannot be earlier than the current date"));
                }

                if(request.Status == Constants.DONE)
                {
                    entity.EndDate = (DateTime)request.EndDate.Value.Date.AddHours(-7);
                } else
                {
                    entity.EndDate = (DateTime)request.EndDate.Value.Date.AddHours(-7).AddDays(1);
                }
            }

            if(user.Id != entity.ReporterId && userProject.RoleId == Constants.MEMBER)
            {
                return Result.Failure(new Error("401", "You don't have permission to update task"));
            }

            if(request.Description != null && request.Description != entity.Description)
            {
                entity.Description = (string)request.Description;
            }

            entity.Status = request.Status;
            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _taskRepository.Update(entity);

            return Result.Success();
        }

        public async Task<Result> Handle(CreatePersonalTaskCommand request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if(email == null)
            {
                return Result.Failure(new Error("401", "Unauthorize"));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Result.Failure(new Error("404", "Not found"));
            }

            var entity = _mapper.Map<Tasks>(request);

            entity.Id = Guid.NewGuid();
            entity.UserId = user.Id;
            entity.ReporterId = user.Id;
            entity.CreatedDate = DateTime.Now.ToUniversalTime();
            entity.IsDelete = Constants.IS_DELETE;

            _taskRepository.Add(entity);

            await _logService.CreateLog("POST", "Add new personal task", "TASK", null, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success();
        }

        public async Task<Result> Handle(UpdatePersonalTaskCommand request, CancellationToken cancellationToken)
        {
            var entity = await _taskRepository.FindByIdAsync(request.Id);

            if(entity == null)
            {
                return Result.Failure(new Error("400", "Record not found"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            if (request.EndDate < request.StartDate)
            {
                return Result.Failure(new Error("400", "EndDate cannot be earlier than the start date"));
            }

            if (entity.Name != request.Name)
            {
                entity.Name = request.Name;
            }

            if(request.EndDate != entity.EndDate)
            {
                entity.EndDate = request.EndDate;
            }

            if(request.StartDate != entity.StartDate)
            {
                entity.StartDate = request.StartDate;
            }

            if(request.Description != entity.Description)
            {
                entity.Description = request.Description;
            }

            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _taskRepository.Update(entity);

            await _logService.CreateLog("PUT", "Update personal task", "TASK", oldEntity, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success();
        }
    }
}
