namespace MVC.DAL.Entities.Accounts
{
    // كود تأكيد رقم التليفون - جدول PhoneVerificationCodes
    // كود واحد شغال لكل مستخدم، والكود نفسه مش بيتخزن (الـ hash بس)
    public class PhoneVerificationCode
    {
        public Guid PhoneVerificationCodeId { get; set; }

        public string UserId { get; set; } = null!; // FK لـ AspNetUsers

        public string CodeHash { get; set; } = null!; // hash الكود

        public string TargetPhoneNumber { get; set; } = null!; // الرقم اللي بيتأكد

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء

        public DateTimeOffset ExpiresAtUtc { get; set; } // وقت الانتهاء (10 دقايق)

        public int FailedAttempts { get; set; } // عدد المحاولات الغلط (أقصى 5)

        public DateTimeOffset? UsedAtUtc { get; set; } // وقت الاستخدام - null لو متستخدمش

        public DateTimeOffset? InvalidatedAtUtc { get; set; } // وقت الإلغاء لما يتطلب كود جديد

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
    }
}
