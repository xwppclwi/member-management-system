using MemberManagement.Models;

namespace MemberManagement.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    public string MemberNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public MemberLevel Level { get; set; }
    public int Points { get; set; }
    public MemberStatus Status { get; set; }
    public string? Address { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string LevelName => Level switch
    {
        MemberLevel.Normal => "普通",
        MemberLevel.Silver => "银卡",
        MemberLevel.Gold => "金卡",
        MemberLevel.Diamond => "钻石",
        _ => "普通"
    };
    public string StatusName => Status switch
    {
        MemberStatus.Active => "正常",
        MemberStatus.Frozen => "冻结",
        MemberStatus.Cancelled => "注销",
        _ => "正常"
    };
}

public class CreateMemberRequest
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Gender { get; set; } = 0;
    public DateTime? Birthday { get; set; }
    public MemberLevel Level { get; set; } = MemberLevel.Normal;
    public string? Address { get; set; }
    public string? Remark { get; set; }
}

public class UpdateMemberRequest
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public MemberLevel Level { get; set; }
    public MemberStatus Status { get; set; }
    public string? Address { get; set; }
    public string? Remark { get; set; }
}

public class MemberQueryParams
{
    public string? Keyword { get; set; }        // 搜索关键词（姓名/手机号/卡号）
    public MemberLevel? Level { get; set; }     // 等级筛选
    public MemberStatus? Status { get; set; }   // 状态筛选
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
