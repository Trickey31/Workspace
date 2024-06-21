using FluentValidation;

namespace Workspace.Contract
{
    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailCommandValidator()
        {
            RuleFor(x => x.ToEmail).NotEmpty().EmailAddress();
        }
    }
}
