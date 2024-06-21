namespace Workspace.Contract
{
    public sealed class ValidationResult : Result, IValidationResult
    {
        private ValidationResult(Error[] errors) 
            : base(false, IValidationResult.ValidationError)
            => Errors = errors;

        public Error[] Errors { get; }

        public static ValidationResult WithErrors(Error[] errors) => new(errors);
    }

    public sealed class TValidationResult<TValue> : TResult<TValue>, IValidationResult
    {
        private TValidationResult(Error[] errors)
            : base(default, false, IValidationResult.ValidationError)
            => Errors = errors;

        public Error[] Errors { get; }

        public static TValidationResult<TValue> WithErrors(Error[] errors) => new(errors);
    }
}
