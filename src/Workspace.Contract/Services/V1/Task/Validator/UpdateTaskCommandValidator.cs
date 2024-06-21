using FluentValidation;

namespace Workspace.Contract
{
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator() 
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ReporterId).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
        }
    }
}
