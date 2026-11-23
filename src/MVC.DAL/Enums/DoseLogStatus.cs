namespace MVC.DAL.Enums
{
    // حالة جرعة الدوا المجدولة للمريض - MedicationDoseLogs.Status
    public enum DoseLogStatus
    {
        Pending, // الجرعة لسه متاخدتش
        Taken, // المريض داس إنه أخدها
        Cancelled // الجرعة اتلغت (إيقاف أو تجديد أو Finish)
    }
}
