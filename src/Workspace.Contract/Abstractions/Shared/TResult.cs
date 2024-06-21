namespace Workspace.Contract
{
    public class TResult<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal TResult(TValue value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure can not be access");

        public static implicit operator TResult<TValue>(TValue? value) => Create(value); 
    }
}
