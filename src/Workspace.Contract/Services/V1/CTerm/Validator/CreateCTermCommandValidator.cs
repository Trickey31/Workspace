using FluentValidation;

namespace Workspace.Contract
{
    public class CreateCTermCommandValidator : AbstractValidator<CreateCTermCommand>
    {
        public CreateCTermCommandValidator() 
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.ProjectId).NotEmpty();
        }
    }
}
