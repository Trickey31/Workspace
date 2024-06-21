using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class ProjectCommandHandler : ICommandHandler<CreateProjectCommand, ProjectCommandResponse>,
                                         ICommandHandler<UpdateProjectCommand, ProjectCommandResponse>,
                                         ICommandHandler<DeleteProjectCommand>,
                                         ICommandHandler<AccessProjectCommand>
    {
        private readonly IRepositoryBase<Project, Guid> _projectRepository;
        private readonly IRepositoryBase<Users_Projects, Guid> _userProjectRepository;
        private readonly IRepositoryBase<CTerm, Guid> _ctermRepository;
        private readonly IRepositoryBase<Parent_CTerm, Guid> _parentCTermRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogService _logService;

        public ProjectCommandHandler(IRepositoryBase<Project, Guid> projectRepository, IMapper mapper,
                                     ApplicationDbContext context, IRepositoryBase<Users_Projects, Guid> userProjectRepository,
                                     UserManager<User> userManager, IRepositoryBase<CTerm, Guid> ctermRepository, IRepositoryBase<Parent_CTerm, Guid> parentCTermRepository, IUserService userService, ILogService logService)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _context = context;
            _userProjectRepository = userProjectRepository;
            _userManager = userManager;
            _ctermRepository = ctermRepository;
            _parentCTermRepository = parentCTermRepository;
            _userService = userService;
            _logService = logService;
        }

        public async Task<TResult<ProjectCommandResponse>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUserAsync();

            var menu = _ctermRepository.FindAll(x => x.Type == "C_MENU" && x.IsDelete == Constants.IS_DELETE);

            var entity = _mapper.Map<Project>(request);

            entity.Id = Guid.NewGuid();
            entity.IsDelete = Constants.IS_DELETE;
            entity.Slug = CommonExtensions.GenerateSlug(request.Name);
            entity.ImgLink = "wwwroot/ObjKey_1/2.png";

            _projectRepository.Add(entity);

            await _context.SaveChangesAsync();

            var userProject = new Users_Projects
            {
                Id = Guid.NewGuid(),
                UserId = user.Value.Id,
                ProjectId = entity.Id,
                RoleId = Constants.ADMINISTRATOR,
                IsDelete = Constants.IS_DELETE,
            };

            _userProjectRepository.Add(userProject);
            await _context.SaveChangesAsync();

            var cterm = new CTerm
            {
                Id = Guid.NewGuid(),
                Name = "Task",
                CssClass = "wwwroot/ObjKey_2/10318.png",
                Type = "C_TASK",
                IsDelete = Constants.IS_DELETE,
            };

            _ctermRepository.Add(cterm);
            await _context.SaveChangesAsync();

            var parent_cterm = new Parent_CTerm
            {
                Id = Guid.NewGuid(),
                ParentId = entity.Id,
                TypeId = cterm.Id,
            };
            _parentCTermRepository.Add(parent_cterm);
            await _context.SaveChangesAsync();

            foreach(var item in menu)
            {
                var menu_cterm = new Parent_CTerm
                {
                    Id = Guid.NewGuid(),
                    ParentId = entity.Id,
                    TypeId = item.Id,
                };

                _parentCTermRepository.Add(menu_cterm);
            }
            await _context.SaveChangesAsync();

            await _logService.CreateLog("POST", "Add new project", "PROJECT", null, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success(new ProjectCommandResponse(entity.Id, entity.Slug));
        }

        public async Task<TResult<ProjectCommandResponse>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _projectRepository.FindByIdAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                return Result.Failure<ProjectCommandResponse>(new Error("400", $"Id {request.Id} không tồn tại"));
            }

            var user = await _userService.GetCurrentUserAsync();

            var userProject = await _userProjectRepository.FindSingleAsync(x => x.UserId == user.Value.Id && x.ProjectId == entity.Id && x.IsDelete == Constants.IS_DELETE);

            if(userProject.RoleId != Constants.ADMINISTRATOR)
            {
                return Result.Failure<ProjectCommandResponse>(new Error("401", "You don't have permission to update this project"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            _mapper.Map(request, entity);

            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _projectRepository.Update(entity);

            await _logService.CreateLog("PUT", "Update project", "PROJECT", oldEntity, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success(new ProjectCommandResponse(entity.Id, entity.Slug));
        }

        public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _projectRepository.FindByIdAsync(request.Id);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", $"Id {request.Id} not found"));
            }

            var user = await _userService.GetCurrentUserAsync();

            var userProject = await _userProjectRepository.FindSingleAsync(x => x.UserId == user.Value.Id && x.ProjectId == entity.Id && x.IsDelete == Constants.IS_DELETE);

            if (userProject.RoleId != Constants.ADMINISTRATOR)
            {
                return Result.Failure<ProjectCommandResponse>(new Error("401", "You don't have permission to update this project"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            entity.IsDelete = Constants.DELETED;
            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _projectRepository.Update(entity);

            await _logService.CreateLog("DELETE", "Delete project", "PROJECT", oldEntity, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success();
        }

        public async Task<Result> Handle(AccessProjectCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var user_project = await _userProjectRepository.FindSingleAsync(x => x.ProjectId == request.ProjectId && x.UserId == currentUser.Value.Id && x.IsDelete == Constants.IS_DELETE);

            if(user_project.RoleId != Constants.ADMINISTRATOR)
            {
                return Result.Failure(new Error("401", "You don't have permission to add user to this project"));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return Result.Failure(new Error("400", "Email not found"));
            }

            var userProject = new Users_Projects
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProjectId = request.ProjectId,
                RoleId = request.RoleId ?? Constants.MEMBER,
                CreatedDate = DateTime.Now,
                IsDelete = Constants.IS_DELETE
            };

            _userProjectRepository.Add(userProject);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
