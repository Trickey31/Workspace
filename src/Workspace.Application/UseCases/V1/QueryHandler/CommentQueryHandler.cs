using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Workspace.Contract;
using Workspace.Domain;

namespace Workspace.Application
{
    public class CommentQueryHandler : IQueryHandler<GetCommentByTaskQuery, List<CommentResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryBase<Comment, Guid> _commentRepository;
        private readonly UserManager<User> _userManager;

        public CommentQueryHandler(IMapper mapper, IRepositoryBase<Comment, Guid> commentRepository, UserManager<User> userManager)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _userManager = userManager;
        }

        public async Task<TResult<List<CommentResponse>>> Handle(GetCommentByTaskQuery request, CancellationToken cancellationToken)
        {
            var users = _userManager.Users;
            var entities = _commentRepository.FindAll(x => x.TaskId == request.TaskId);

            var result = from a in entities
                         join b in users on a.UserId equals b.Id
                         select new CommentResponse
                         {
                             Id = a.Id,
                             Name = b.Name,
                             ImgLink = b.ImgLink,
                             TaskId = a.TaskId,
                             UserId = a.UserId,
                             Content = a.Content,
                             CreatedDate = a.CreatedDate,
                             UpdatedDate = a.UpdatedDate,
                         };

            return result.ToList();
        }
    }
}
