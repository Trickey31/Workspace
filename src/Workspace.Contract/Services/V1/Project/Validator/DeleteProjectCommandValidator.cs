using FluentValidation;

namespace Workspace.Contract
{
    public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
    {
        public DeleteProjectCommandValidator() 
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
