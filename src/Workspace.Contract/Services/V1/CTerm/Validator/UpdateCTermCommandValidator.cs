using FluentValidation;

namespace Workspace.Contract
{
    public class UpdateCTermCommandValidator : AbstractValidator<UpdateCTermCommand>
    {
        public UpdateCTermCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
