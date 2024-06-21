//using Workspace.Contract;

//namespace Workspace.Application
//{
//    internal class SendEmailWhenUserRegistedEventHandler : IDomainEventHandler<UserDomainEvent.UserRegisted>
//    {
//        private readonly IEmailService _emailService;

//        public async Task Handle(UserDomainEvent.UserRegisted notification, CancellationToken cancellationToken)
//        {
//            await _emailService.SendEmailAsync()
//        }
//    }
//}
