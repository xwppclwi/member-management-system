namespace MemberManagement.Common;

public class JwtSettings
{
    public string Secret { get; set; } = "your-super-secret-key-with-at-least-32-characters-long!";
    public string Issuer { get; set; } = "MemberManagement";
    public string Audience { get; set; } = "MemberManagementClient";
    public int ExpirationHours { get; set; } = 24;
}
