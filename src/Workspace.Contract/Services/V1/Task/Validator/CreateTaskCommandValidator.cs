using FluentValidation;

namespace Workspace.Contract
{
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ReporterId).NotEmpty();
        }
    }
}
