namespace Workspace.Contract
{
    public class Result
    {
        public bool IsSuccess { get; }

        public bool IsFailed => !IsSuccess;

        public Error Error { get; }

        protected internal Result(bool isSucess, Error error) 
        {
            if(isSucess && error != Error.None)
            {
                throw new InvalidOperationException();
            }

            if (!isSucess && error == Error.None)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSucess;
            Error = error;
        }

        public static Result Success() => 
            new(true, Error.None);

        public static TResult<TValue> Success<TValue>(TValue value) =>
            new(value, true, Error.None);

        public static Result Failure(Error error) => 
            new(false, error);

        public static TResult<TValue> Failure<TValue>(Error error) =>
            new(default, false, error);

        public static TResult<TValue> Create<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }
}
