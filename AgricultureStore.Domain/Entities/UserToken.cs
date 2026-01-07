namespace AgricultureStore.Domain.Entities
{
    public enum TokenType
    {
        EmailVerification = 1,
        PasswordReset = 2,
        RefreshToken = 3
    }

    public class UserToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public TokenType TokenType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        
        // For refresh tokens - store device/client info
        public string? DeviceInfo { get; set; }
        public string? IpAddress { get; set; }

        // Navigation property
        public User? User { get; set; }

        // Helper properties
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsUsed && RevokedAt == null && !IsExpired;
    }
}
