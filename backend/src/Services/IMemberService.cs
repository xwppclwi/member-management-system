using MemberManagement.DTOs;
using MemberManagement.Models;

namespace MemberManagement.Services;

public interface IMemberService
{
    Task<MemberDto?> GetByIdAsync(int id);
    Task<MemberDto?> GetByMemberNoAsync(string memberNo);
    Task<PagedResult<MemberDto>> GetPagedAsync(MemberQueryParams query);
    Task<MemberDto> CreateAsync(CreateMemberRequest request, int createdBy);
    Task<MemberDto?> UpdateAsync(int id, UpdateMemberRequest request);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByPhoneAsync(string phone);
    Task<int> GetMemberPointsAsync(int memberId);
}
