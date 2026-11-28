namespace MVC.DAL.Enums
{
    // Type of exception on a shift on a specific date - ShiftExceptions.ExceptionType
    public enum ShiftExceptionType
    {
        Blocked, // The shift is cancelled on this date
        SpecialWorkingHours // Different hours on this date (same number of patients)
    }
}
