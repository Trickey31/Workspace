using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class CTermQueryHandler : IQueryHandler<GetListCTermByTypeAndProjectQuery, List<CTermResponse>>,
                                     IQueryHandler<GetListCTermByTypeQuery, List<CTermResponse>>,
                                     IQueryHandler<GetListCTermByCurrentUserQuery, List<CTermResponse>>,
                                     IQueryHandler<GetByIdQuery, CTermResponse>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryBase<CTerm, Guid> _cTermRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IRepositoryBase<Users_Projects, Guid> _userProjectRepository;
        private readonly IRepositoryBase<Project, Guid> _projectRepository;
        private readonly IRepositoryBase<Parent_CTerm, Guid> _projectCTermRepository;

        public CTermQueryHandler(IMapper mapper, IRepositoryBase<CTerm, Guid> cTermRepository, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IRepositoryBase<Users_Projects, Guid> userProjectRepository, IRepositoryBase<Project, Guid> projectRepository, IRepositoryBase<Parent_CTerm, Guid> projectCTermRepository)
        {
            _mapper = mapper;
            _cTermRepository = cTermRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _userProjectRepository = userProjectRepository;
            _projectRepository = projectRepository;
            _projectCTermRepository = projectCTermRepository;
        }

        public async Task<TResult<List<CTermResponse>>> Handle(GetListCTermByTypeAndProjectQuery request, CancellationToken cancellationToken)
        {
            var project_cterm = _projectCTermRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE && x.ParentId == request.ProjectId);

            var cterm = _cTermRepository.FindAll(x => x.Type.ToLower() == request.Type.ToLower() && x.IsDelete == Constants.IS_DELETE).OrderBy(x => x.CreatedDate);

            var query = from a in cterm
                        join b in project_cterm on a.Id equals b.TypeId
                        select new CTermResponse
                        {
                            Id = a.Id,
                            Name = a.Name,
                            CssClass = a.CssClass,
                            Description = a.Description,
                            Type = a.Type,
                            Slug = ConvertToSlug(a.Name),
                        };

            if (query == null)
            {
                throw new NotFoundException("Không tìm thấy bản ghi");
            }

            return query.ToList();
        }

        public async Task<TResult<List<CTermResponse>>> Handle(GetListCTermByTypeQuery request, CancellationToken cancellationToken)
        {
            var cterm = _cTermRepository.FindAll(x => x.Type.ToLower() == request.Type.ToLower() && x.IsDelete == Constants.IS_DELETE);

            if (cterm == null)
            {
                throw new NotFoundException("Không tìm thấy bản ghi");
            }

            var response = _mapper.Map<List<CTermResponse>>(cterm);

            return response;
        }

        public async Task<TResult<List<CTermResponse>>> Handle(GetListCTermByCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if(email == null)
            {
                throw new NotFoundException("Email invalid");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                throw new NotFoundException("User invalid");
            }

            var user_project = _userProjectRepository.FindAll(x => x.UserId == user.Id && x.IsDelete == Constants.IS_DELETE);

            var project = _projectRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var parent_cterm = _projectCTermRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var cterm = _cTermRepository.FindAll(x => x.IsDelete == Constants.IS_DELETE);

            var query = from a in cterm
                        join b in parent_cterm on a.Id equals b.TypeId
                        join c in project on b.ParentId equals c.Id
                        join d in user_project on c.Id equals d.ProjectId
                        select new CTermResponse
                        {
                            Id = a.Id,
                            Name = a.Name,
                        };

            var response = query.ToList();

            return response;
        }

        public async Task<TResult<CTermResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _cTermRepository.FindSingleAsync(x => x.Id == request.Id);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                throw new NotFoundException("Id invalid");
            }

            var res = _mapper.Map<CTermResponse>(entity);

            return res;
        }

        private static string ConvertToSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Input string cannot be null or whitespace.", nameof(name));
            }

            // Chuyển chuỗi về chữ thường
            string lowerCaseName = name.ToLower();

            // Thay thế các khoảng trắng và các ký tự đặc biệt bằng dấu gạch ngang
            string slug = Regex.Replace(lowerCaseName, @"\s+", "-");

            // Loại bỏ các ký tự không hợp lệ (giữ lại các ký tự chữ cái, số và dấu gạch ngang)
            slug = Regex.Replace(slug, @"[^a-z0-9-]", "");

            return slug;
        }
    }
}
