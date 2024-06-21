using Workspace.Contract;
using Workspace.Domain;

namespace Workspace.Application
{
    public class EmailCommandHandler : ICommandHandler<SendEmailCommand>,
                                       ICommandHandler<VerifyEmailCommand>
    {
        private readonly IEmailService _emailService;

        public EmailCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<Result> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(request.ToEmail);

            return Result.Success();
        }

        public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _emailService.VerifyEmailAsync(request.Email, request.Otp);

            if(!result)
            {
                return Result.Failure(new Error("Bad request", "Bad request"));
            }

            return Result.Success();
        }
    }
}
