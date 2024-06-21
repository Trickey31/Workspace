using AutoMapper;
using Newtonsoft.Json;
using System.Security.Principal;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class CTermCommandHandler : ICommandHandler<CreateCTermCommand>,
                                       ICommandHandler<UpdateCTermCommand>,
                                       ICommandHandler<DeleteCTermCommand>
    {
        private readonly IRepositoryBase<CTerm, Guid> _ctermRepository;
        private readonly IRepositoryBase<Parent_CTerm, Guid> _parentRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public CTermCommandHandler(IRepositoryBase<CTerm, Guid> ctermRepository, IRepositoryBase<Parent_CTerm, Guid> parentRepository, IMapper mapper, ApplicationDbContext context, ILogService logService)
        {
            _ctermRepository = ctermRepository;
            _parentRepository = parentRepository;
            _mapper = mapper;
            _context = context;
            _logService = logService;
        }

        public async Task<Result> Handle(CreateCTermCommand request, CancellationToken cancellationToken)
        {
            var typeId = _parentRepository.FindAll(x => x.ParentId == request.ProjectId && x.IsDelete == Constants.IS_DELETE).Select(x => x.TypeId);

            var exit = await _ctermRepository.FindSingleAsync(x => x.Name == request.Name && typeId.Contains(x.Id) && x.IsDelete == Constants.IS_DELETE);

            if (exit != null)
            {
                return Result.Failure(new Error("400", "Record is exist"));
            }

            var entity = _mapper.Map<CTerm>(request);
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.Now.ToUniversalTime();
            entity.IsDelete = Constants.IS_DELETE;

            _ctermRepository.Add(entity);
            await _context.SaveChangesAsync();

            var parent = new Parent_CTerm
            {
                Id = Guid.NewGuid(),
                ParentId = request.ProjectId,
                TypeId = entity.Id,
                CreatedDate = DateTime.Now.ToUniversalTime(),
                IsDelete = Constants.IS_DELETE,
            };

            _parentRepository.Add(parent);
            await _context.SaveChangesAsync();

            await _logService.CreateLog("POST", "Add new cterm", "CTERM", null, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success();
        }

        public async Task<Result> Handle(UpdateCTermCommand request, CancellationToken cancellationToken)
        {
            var entity = await _ctermRepository.FindByIdAsync(request.Id);

            if(entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", "Record invalid"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            _mapper.Map(request, entity);
            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _ctermRepository.Update(entity);

            await _logService.CreateLog("PUT", "Update cterm", "CTERM", oldEntity, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success();
        }

        public async Task<Result> Handle(DeleteCTermCommand request, CancellationToken cancellationToken)
        {
            var entity = await _ctermRepository.FindByIdAsync(request.Id);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", "Record invalid"));
            }

            var parent = await _parentRepository.FindSingleAsync(x => x.TypeId == entity.Id && x.IsDelete == Constants.IS_DELETE);

            if (parent == null)
            {
                return Result.Failure(new Error("400", "Record invalid"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            entity.IsDelete = Constants.DELETED;
            _ctermRepository.Update(entity);
            await _context.SaveChangesAsync();

            parent.IsDelete = Constants.DELETED;
            _parentRepository.Update(parent);

            await _context.SaveChangesAsync();

            await _logService.CreateLog("DELETE", "Delete cterm", "CTERM", oldEntity, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success();
        }
    }
}
