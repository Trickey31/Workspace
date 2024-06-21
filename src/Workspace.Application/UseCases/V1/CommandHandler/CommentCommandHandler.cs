using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class CommentCommandHandler : ICommandHandler<CreateCommentCommand>,
                                         ICommandHandler<UpdateCommentCommand>,
                                         ICommandHandler<DeleteCommentCommand>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryBase<Comment, Guid> _commentRepository;
        private readonly ILogService _logService;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IRepositoryBase<Tasks, Guid> _taskRepository;
        private readonly IRepositoryBase<Notification, Guid> _notificationRepository;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IRepositoryBase<Users_Projects, Guid> _userProjectRepository;

        public CommentCommandHandler(IMapper mapper, IRepositoryBase<Comment, Guid> commentRepository, ILogService logService, IUserService userService, ApplicationDbContext context, IRepositoryBase<Tasks, Guid> taskRepository, IRepositoryBase<Notification, Guid> notificationRepository,
                                  IHubContext<NotificationHub> notificationHubContext, IRepositoryBase<Users_Projects, Guid> userProjectRepository)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _logService = logService;
            _userService = userService;
            _context = context;
            _taskRepository = taskRepository;
            _notificationRepository = notificationRepository;
            _notificationHubContext = notificationHubContext;
            _userProjectRepository = userProjectRepository;
        }

        public async Task<Result> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var list = new List<Guid>();

            var task = await _taskRepository.FindByIdAsync(request.TaskId);

            if (task == null)
            {
                return Result.Failure(new Error("400", "Task not found"));
            }

            var user = await _userService.GetCurrentUserAsync();

            if(user.Value == null)
            {
                return Result.Failure(new Error("401", "Unauthorized"));
            }

            var entity = _mapper.Map<Comment>(request);

            entity.CreatedDate = DateTime.Now.ToUniversalTime();
            entity.UserId = user.Value.Id;

            _commentRepository.Add(entity);
            await _context.SaveChangesAsync();
            await _logService.CreateLog("POST", "Add comment", "COMMENT", null, JsonConvert.SerializeObject(entity), entity.TaskId);

            if(user.Value.Id == task.ReporterId)
            {
                var userProject = _userProjectRepository.FindAll(x => x.UserId == task.UserId && x.ProjectId == task.ProjectId && x.IsDelete == Constants.IS_DELETE);

                if(userProject.Any())
                {
                    list.Add(task.UserId);

                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        FromUser = user.Value.Id,
                        ToUser = task.UserId,
                        FunctionType = "POST",
                        FunctionName = "Add a comment",
                        ObjId = task.Id,
                        Type = Constants.COMMENT_NOTIFICATION,
                        IsNew = true,
                    };

                    _notificationRepository.Add(notification);
                    await _context.SaveChangesAsync();
                    await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", JsonConvert.SerializeObject(list));
                }
            } else if(user.Value.Id == task.UserId)
            {
                var userProject = _userProjectRepository.FindAll(x => x.UserId == task.ReporterId && x.ProjectId == task.ProjectId && x.IsDelete == Constants.IS_DELETE);

                if (userProject.Any())
                {
                    list.Add(task.ReporterId);

                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        FromUser = user.Value.Id,
                        ToUser = task.ReporterId,
                        FunctionType = "POST",
                        FunctionName = "Add a comment",
                        ObjId = task.Id,
                        Type = Constants.COMMENT_NOTIFICATION,
                        IsNew = true,
                    };

                    _notificationRepository.Add(notification);
                    await _context.SaveChangesAsync();
                    await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", JsonConvert.SerializeObject(list));
                }
            } else
            {
                list.Add(task.UserId);
                list.Add(task.ReporterId);

                var listNoti = new List<Notification>();

                var notification1 = new Notification
                {
                    Id = Guid.NewGuid(),
                    FromUser = user.Value.Id,
                    ToUser = task.UserId,
                    FunctionType = "POST",
                    FunctionName = "Add a comment",
                    ObjId = task.Id,
                    Type = Constants.COMMENT_NOTIFICATION,
                    IsNew = true,
                };

                var notification2 = new Notification
                {
                    Id = Guid.NewGuid(),
                    FromUser = user.Value.Id,
                    ToUser = task.ReporterId,
                    FunctionType = "POST",
                    FunctionName = "Add a comment",
                    ObjId = task.Id,
                    Type = Constants.COMMENT_NOTIFICATION,
                    IsNew = true,
                };

                _notificationRepository.Add(notification1);
                _notificationRepository.Add(notification2);
                await _context.SaveChangesAsync();
                await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", JsonConvert.SerializeObject(list));
            }

            return Result.Success();
        }

        public async Task<Result> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUserAsync();

            if (user.Value == null)
            {
                return Result.Failure(new Error("401", "Unauthorized"));
            }

            var entity = await _commentRepository.FindByIdAsync(request.Id);

            if(entity == null)
            {
                return Result.Failure(new Error("400", "Record not found"));
            }

            if(entity.UserId !=  user.Value.Id)
            {
                return Result.Failure(new Error("400", "Bad request"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            entity.Content = request.Content;
            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _commentRepository.Update(entity);
            await _logService.CreateLog("PUT", "Update comment", "COMMENT", oldEntity,JsonConvert.SerializeObject(entity), entity.TaskId);

            return Result.Success();
        }

        public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUserAsync();

            if (user.Value == null)
            {
                return Result.Failure(new Error("401", "Unauthorized"));
            }

            var entity = await _commentRepository.FindByIdAsync(request.Id);

            if (entity == null)
            {
                return Result.Failure(new Error("400", "Record not found"));
            }

            if (entity.UserId != user.Value.Id)
            {
                return Result.Failure(new Error("400", "Bad request"));
            }

            _commentRepository.Remove(entity);

            await _logService.CreateLog("DELETE", "Delete comment", "COMMENT", JsonConvert.SerializeObject(entity), null, entity.TaskId);

            return Result.Success();
        }
    }
}
