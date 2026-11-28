namespace MVC.BLL.Common
{
    // Result of any service operation: success or not + error messages
    // Messages are in English, and the PL translates them with the localizer (L[error])
    public class ServiceResult
    {
        public bool Succeeded { get; protected init; }
        public IReadOnlyList<string> Errors { get; protected init; } = Array.Empty<string>();

        public static ServiceResult Success() => new() { Succeeded = true };

        public static ServiceResult Fail(params string[] errors) => new() { Succeeded = false, Errors = errors };

        public static ServiceResult Fail(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors.ToList() };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Value { get; private init; }

        public static ServiceResult<T> Success(T value) => new() { Succeeded = true, Value = value };

        public static new ServiceResult<T> Fail(params string[] errors) => new() { Succeeded = false, Errors = errors };

        public static new ServiceResult<T> Fail(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors.ToList() };
    }
}