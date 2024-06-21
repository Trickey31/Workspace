using FluentValidation;

namespace Workspace.Contract
{
    public class DeleteCTermCommandValidator : AbstractValidator<DeleteCTermCommand>
    {
        public DeleteCTermCommandValidator() 
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
