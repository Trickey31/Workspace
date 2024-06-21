using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class NotificationQueryHandler : IQueryHandler<GetNotificationsByUserIdQuery, List<NotificationResponse>>
    {
        private readonly IRepositoryBase<Notification, Guid> _notiRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IRepositoryBase<Tasks, Guid> _taskRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryBase<Project, Guid> _projectRepository;
        private readonly IRepositoryBase<CTerm, Guid> _ctermRepository;
        private readonly IRepositoryBase<Parent_CTerm, Guid> _parentCTermRepository;

        public NotificationQueryHandler(IRepositoryBase<Notification, Guid> notiRepository, IMapper mapper, IUserService userService, IRepositoryBase<Tasks, Guid> taskRepository, UserManager<User> userManager, IRepositoryBase<Project, Guid> projectRepository, IRepositoryBase<CTerm, Guid> ctermRepository, IRepositoryBase<Parent_CTerm, Guid> parentCTermRepository)
        {
            _notiRepository = notiRepository;
            _mapper = mapper;
            _userService = userService;
            _taskRepository = taskRepository;
            _userManager = userManager;
            _projectRepository = projectRepository;
            _ctermRepository = ctermRepository;
            _parentCTermRepository = parentCTermRepository;
        }

        public async Task<TResult<List<NotificationResponse>>> Handle(GetNotificationsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUserAsync();

            var taskNoti = _notiRepository.FindAll(x => x.ToUser == user.Value.Id);

            var task = _taskRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ProjectId != null);

            var cterm = _ctermRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var project = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var allUsers = _userManager.Users;

            var taskNotiQuery = from a in taskNoti
                               join b in allUsers on a.FromUser equals b.Id
                               join c in task on a.ObjId equals c.Id
                               join d in cterm on c.Type equals d.Id
                               join e in project on c.ProjectId equals e.Id
                               select new NotificationResponse
                               {
                                   Id = a.Id,
                                   FromUser = b.Id,
                                   FromUserName = b.Name,
                                   ToUser = user.Value.Id,
                                   FunctionType = a.FunctionType,
                                   FunctionName = a.FunctionName,
                                   ObjId = c.Id,
                                   ObjName = c.Name,
                                   Icon = b.ImgLink,
                                   CreatedDate = a.CreatedDate,
                                   ParentObjName = e.Name,
                                   ParentObjSlug = e.Slug,
                                   HaveSeen = a.HaveSeen,
                                   IsNew = a.IsNew,
                               };


            var response = taskNotiQuery.OrderByDescending(x => x.CreatedDate).ToList();

            return response;
        }
    }
}
