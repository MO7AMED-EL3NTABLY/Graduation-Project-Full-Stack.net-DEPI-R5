namespace MVC.DAL.Entities.Accounts
{
    // Phone number verification code - PhoneVerificationCodes table
    // One active code per user, and the code itself is never stored (only its hash)
    public class PhoneVerificationCode
    {
        public Guid PhoneVerificationCodeId { get; set; }

        public string UserId { get; set; } = null!; // FK to AspNetUsers

        public string CodeHash { get; set; } = null!; // Hash of the code

        public string TargetPhoneNumber { get; set; } = null!; // The number being verified

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time

        public DateTimeOffset ExpiresAtUtc { get; set; } // Expiry time (10 minutes)

        public int FailedAttempts { get; set; } // Number of failed attempts (max 5)

        public DateTimeOffset? UsedAtUtc { get; set; } // Time used - null if not used

        public DateTimeOffset? InvalidatedAtUtc { get; set; } // Invalidation time when a new code is requested

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
    }
}
