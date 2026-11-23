namespace MVC.DAL.Enums
{
    // حالة الحجز - Appointments.Status
    public enum AppointmentStatus
    {
        Confirmed, // الحجز متأكد ولسه الزيارة متمتش
        Completed, // الزيارة تمت (أول إجراء روشتة)
        Cancelled, // الحجز اتلغى
        Missed // المريض محضرش (آخر اليوم بتوقيت القاهرة)
    }
}
