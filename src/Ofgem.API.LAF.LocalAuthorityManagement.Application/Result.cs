namespace Ofgem.API.LAF.LocalAuthorityManagement.Application
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        
        public T? Value { get; private set; }

        public string[]? Errors { get; private set; }

        private Result() { }

        public static Result<T> Success(T value) => new()
        {
            IsSuccess = true,
            Value = value
        };

        public static Result<T> Failure(params string[] errors) => new()
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}
