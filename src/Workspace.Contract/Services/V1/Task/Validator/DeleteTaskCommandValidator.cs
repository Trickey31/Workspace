using FluentValidation;

namespace Workspace.Contract
{
    public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
    {
        public DeleteTaskCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
