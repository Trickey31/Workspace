using FluentValidation;

namespace Workspace.Contract
{
    public class DeleteTaskValidator : AbstractValidator<Commands.DeleteTask>
    {
        public DeleteTaskValidator() 
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
