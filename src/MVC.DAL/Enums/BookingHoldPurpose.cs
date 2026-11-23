namespace MVC.DAL.Enums
{
    // سبب حجز المكان المؤقت (دقيقتين) - BookingHolds.Purpose
    public enum BookingHoldPurpose
    {
        NewBooking, // حجز جديد
        Reschedule // تغيير ميعاد حجز موجود
    }
}
