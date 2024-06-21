using Workspace.Contract;
using Workspace.Domain;

namespace Workspace.Application
{
    public class NotificationCommandHandler : ICommandHandler<UpdateIsNewCommand>
    {
        private readonly IRepositoryBase<Notification, Guid> _notiRepository;
        private readonly IUserService _userService;

        public NotificationCommandHandler(IRepositoryBase<Notification, Guid> notiRepository, IUserService userService)
        {
            _notiRepository = notiRepository;
            _userService = userService;
        }

        public async Task<Result> Handle(UpdateIsNewCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUserAsync();

            var query = _notiRepository.FindAll(x => x.ToUser == user.Value.Id);

            if (query.Any())
            {
                foreach (var item in query)
                {
                    if (item.IsNew)
                    {
                        item.IsNew = false;
                        _notiRepository.Update(item);
                    }
                }
            }

            return Result.Success();
        }
    }
}
