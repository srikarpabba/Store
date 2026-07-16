using SharedKernel;

namespace Domain.Users;

public sealed class RefreshToken : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOnUtc { get; set; }
    public bool IsRevoked => RevokedOnUtc.HasValue;
    public DateTime? RevokedOnUtc { get; private set; }
    public string? DeviceName { get; set; }
    public string? IpAddress { get; set; }
    public void Revoke(DateTime revokedOnUtc)
    {
        RevokedOnUtc = revokedOnUtc;
    }
}
