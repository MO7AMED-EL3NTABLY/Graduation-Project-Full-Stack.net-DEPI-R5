/*——- Omar Hussein ——-*/
namespace MVC.BLL.Common
{
    // نتيجة أي عملية في الـ Services: نجحت ولا لأ + رسايل الأخطاء
    // الرسايل بالإنجليزي، والـ PL بيترجمها بالـ Localizer (L[error])
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
/*————*/
