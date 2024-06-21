using FluentValidation;

namespace Workspace.Contract
{
    public class CreatePersonalTaskCommandValidator : AbstractValidator<CreatePersonalTaskCommand>
    {
        public CreatePersonalTaskCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
        }
    }
}
