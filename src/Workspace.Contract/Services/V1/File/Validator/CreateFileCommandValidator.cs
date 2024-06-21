using FluentValidation;

namespace Workspace.Contract
{
    public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
    {
        public CreateFileCommandValidator() 
        {
            RuleFor(x => x.File).NotEmpty();
            RuleFor(x => x.ObjId).NotEmpty();
            RuleFor(x => x.ObjKey).NotEmpty();
        }
    }
}
