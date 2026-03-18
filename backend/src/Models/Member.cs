namespace MemberManagement.Models;

public enum MemberLevel
{
    Normal = 0,     // 普通
    Silver = 1,     // 银卡
    Gold = 2,       // 金卡
    Diamond = 3     // 钻石
}

public enum MemberStatus
{
    Active = 0,     // 正常
    Frozen = 1,     // 冻结
    Cancelled = 2   // 注销
}

public class Member
{
    public int Id { get; set; }
    public string MemberNo { get; set; } = string.Empty;    // 会员卡号
    public string Name { get; set; } = string.Empty;        // 姓名
    public string Phone { get; set; } = string.Empty;       // 手机号
    public string? Email { get; set; }                      // 邮箱
    public int Gender { get; set; } = 0;                    // 性别: 0-未知, 1-男, 2-女
    public DateTime? Birthday { get; set; }                 // 生日
    public MemberLevel Level { get; set; } = MemberLevel.Normal;  // 等级
    public int Points { get; set; } = 0;                    // 当前积分
    public MemberStatus Status { get; set; } = MemberStatus.Active; // 状态
    public string? Address { get; set; }                    // 地址
    public string? Remark { get; set; }                     // 备注
    public bool IsDeleted { get; set; } = false;            // 软删除标记
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
