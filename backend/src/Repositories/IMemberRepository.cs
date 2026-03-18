using MemberManagement.Models;

namespace MemberManagement.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(int id);
    Task<Member?> GetByMemberNoAsync(string memberNo);
    Task<Member?> GetByPhoneAsync(string phone);
    Task<(List<Member> Items, int TotalCount)> GetPagedAsync(
        string? keyword,
        MemberLevel? level,
        MemberStatus? status,
        int pageNumber,
        int pageSize);
    Task<Member> CreateAsync(Member member);
    Task<Member> UpdateAsync(Member member);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> ExistsByPhoneAsync(string phone);
    Task<bool> ExistsByEmailAsync(string email);
    Task<int> GetNextMemberNoAsync();
}
