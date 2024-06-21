using FluentValidation;

namespace Workspace.Contract
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.ConfirmPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            RuleFor(x => x.OldPassword).NotEmpty();
        }
    }
}
