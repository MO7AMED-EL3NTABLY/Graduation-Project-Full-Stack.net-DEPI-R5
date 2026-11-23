namespace MVC.DAL.Entities.Accounts
{
    // توكن تجديد تسجيل الدخول - جدول RefreshTokens
    // بيتغير مع كل استخدام (rotation) وبيتخزن الـ hash بس
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }

        public string UserId { get; set; } = null!; // FK لـ AspNetUsers

        public string TokenHash { get; set; } = null!; // hash التوكن - التوكن نفسه مش بيتخزن

        public DateTimeOffset CreatedAtUtc { get; set; } // وقت الإنشاء

        public DateTimeOffset ExpiresAtUtc { get; set; } // وقت الانتهاء (7 أيام)

        public DateTimeOffset? RevokedAtUtc { get; set; } // وقت الإلغاء - null لو لسه شغال

        public Guid? ReplacedByTokenId { get; set; } // التوكن اللي حل مكانه (self-reference)

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public RefreshToken? ReplacedByToken { get; set; }
    }
}
