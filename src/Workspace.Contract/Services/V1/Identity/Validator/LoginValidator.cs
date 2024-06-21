using FluentValidation;

namespace Workspace.Contract
{
    public class LoginValidator : AbstractValidator<AuthenCommand.Login>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
