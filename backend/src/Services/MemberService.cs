using MemberManagement.DTOs;
using MemberManagement.Models;
using MemberManagement.Repositories;

namespace MemberManagement.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<MemberDto?> GetByIdAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        return member == null ? null : MapToDto(member);
    }

    public async Task<MemberDto?> GetByMemberNoAsync(string memberNo)
    {
        var member = await _memberRepository.GetByMemberNoAsync(memberNo);
        return member == null ? null : MapToDto(member);
    }

    public async Task<PagedResult<MemberDto>> GetPagedAsync(MemberQueryParams query)
    {
        var (items, totalCount) = await _memberRepository.GetPagedAsync(
            query.Keyword,
            query.Level,
            query.Status,
            query.PageNumber,
            query.PageSize);

        return new PagedResult<MemberDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task<MemberDto> CreateAsync(CreateMemberRequest request, int createdBy)
    {
        // 生成会员卡号
        var nextNo = await _memberRepository.GetNextMemberNoAsync();
        var memberNo = $"M{nextNo:D6}";

        var member = new Member
        {
            MemberNo = memberNo,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Gender = request.Gender,
            Birthday = request.Birthday,
            Level = request.Level,
            Points = 0,
            Status = MemberStatus.Active,
            Address = request.Address,
            Remark = request.Remark,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _memberRepository.CreateAsync(member);
        return MapToDto(created);
    }

    public async Task<MemberDto?> UpdateAsync(int id, UpdateMemberRequest request)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) return null;

        member.Name = request.Name;
        member.Phone = request.Phone;
        member.Email = request.Email;
        member.Gender = request.Gender;
        member.Birthday = request.Birthday;
        member.Level = request.Level;
        member.Status = request.Status;
        member.Address = request.Address;
        member.Remark = request.Remark;
        member.UpdatedAt = DateTime.UtcNow;

        var updated = await _memberRepository.UpdateAsync(member);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _memberRepository.SoftDeleteAsync(id);
    }

    public async Task<bool> ExistsByPhoneAsync(string phone)
    {
        return await _memberRepository.ExistsByPhoneAsync(phone);
    }

    public async Task<int> GetMemberPointsAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        return member?.Points ?? 0;
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            MemberNo = member.MemberNo,
            Name = member.Name,
            Phone = member.Phone,
            Email = member.Email,
            Gender = member.Gender,
            Birthday = member.Birthday,
            Level = member.Level,
            Points = member.Points,
            Status = member.Status,
            Address = member.Address,
            Remark = member.Remark,
            CreatedAt = member.CreatedAt,
            UpdatedAt = member.UpdatedAt
        };
    }
}
