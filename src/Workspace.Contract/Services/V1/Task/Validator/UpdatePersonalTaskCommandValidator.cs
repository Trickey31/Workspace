using FluentValidation;

namespace Workspace.Contract
{
    public class UpdatePersonalTaskCommandValidator : AbstractValidator<UpdatePersonalTaskCommand>
    {
        public UpdatePersonalTaskCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
        }
    }
}
