namespace MVC.DAL.Enums
{
    // Email or SMS delivery status - Notifications.EmailStatus and SmsStatus
    public enum DeliveryStatus
    {
        NotRequired, // This channel is off and needs no sending
        Pending, // Waiting to be sent
        Sent, // Sent
        Failed // Failed after 3 attempts
    }
}
