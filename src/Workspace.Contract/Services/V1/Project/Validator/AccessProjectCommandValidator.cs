using FluentValidation;

namespace Workspace.Contract
{
    public class AccessProjectCommandValidator : AbstractValidator<AccessProjectCommand>
    {
        public AccessProjectCommandValidator() 
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
