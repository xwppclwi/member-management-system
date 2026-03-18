using MemberManagement.Data;
using MemberManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberManagement.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
    }

    public async Task<Member?> GetByMemberNoAsync(string memberNo)
    {
        return await _context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MemberNo == memberNo && !m.IsDeleted);
    }

    public async Task<Member?> GetByPhoneAsync(string phone)
    {
        return await _context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Phone == phone && !m.IsDeleted);
    }

    public async Task<(List<Member> Items, int TotalCount)> GetPagedAsync(
        string? keyword,
        MemberLevel? level,
        MemberStatus? status,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Members
            .AsNoTracking()
            .Where(m => !m.IsDeleted);

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(m =>
                m.Name.Contains(keyword) ||
                m.Phone.Contains(keyword) ||
                m.MemberNo.Contains(keyword));
        }

        // 等级筛选
        if (level.HasValue)
        {
            query = query.Where(m => m.Level == level.Value);
        }

        // 状态筛选
        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Member> CreateAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<Member> UpdateAsync(Member member)
    {
        member.UpdatedAt = DateTime.UtcNow;
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null) return false;

        member.IsDeleted = true;
        member.Status = MemberStatus.Cancelled;
        member.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByPhoneAsync(string phone)
    {
        return await _context.Members
            .AnyAsync(m => m.Phone == phone && !m.IsDeleted);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return await _context.Members
            .AnyAsync(m => m.Email == email && !m.IsDeleted);
    }

    public async Task<int> GetNextMemberNoAsync()
    {
        var lastMember = await _context.Members
            .OrderByDescending(m => m.Id)
            .FirstOrDefaultAsync();

        if (lastMember == null)
            return 1;

        // 提取数字部分
        var lastNo = lastMember.MemberNo;
        if (lastNo.StartsWith("M") && int.TryParse(lastNo[1..], out var num))
        {
            return num + 1;
        }

        return lastMember.Id + 1;
    }
}
