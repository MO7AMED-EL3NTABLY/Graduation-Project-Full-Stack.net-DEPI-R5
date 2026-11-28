namespace MVC.DAL.Entities.Accounts
{
    // Sign-in refresh token - RefreshTokens table
    // Changes on every use (rotation) and only the hash is stored
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }

        public string UserId { get; set; } = null!; // FK to AspNetUsers

        public string TokenHash { get; set; } = null!; // Hash of the token - the token itself is never stored

        public DateTimeOffset CreatedAtUtc { get; set; } // Creation time

        public DateTimeOffset ExpiresAtUtc { get; set; } // Expiry time (7 days)

        public DateTimeOffset? RevokedAtUtc { get; set; } // Revocation time - null if still active

        public Guid? ReplacedByTokenId { get; set; } // The token that replaced it (self-reference)

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public RefreshToken? ReplacedByToken { get; set; }
    }
}
