namespace MVC.DAL.Enums
{
    // نوع الاستثناء على شيفت في يوم معين - ShiftExceptions.ExceptionType
    public enum ShiftExceptionType
    {
        Blocked, // الشيفت ملغي في اليوم ده
        SpecialWorkingHours // مواعيد مختلفة في اليوم ده (نفس عدد المرضى)
    }
}
