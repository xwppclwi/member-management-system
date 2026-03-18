using MemberManagement.DTOs;
using MemberManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemberManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberService memberService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _logger = logger;
    }

    /// <summary>
    /// 获取会员列表（分页）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<MemberDto>>> GetList([FromQuery] MemberQueryParams query)
    {
        var result = await _memberService.GetPagedAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// 获取会员详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetById(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member == null)
            return NotFound(new { message = "会员不存在" });

        return Ok(member);
    }

    /// <summary>
    /// 根据卡号获取会员
    /// </summary>
    [HttpGet("by-no/{memberNo}")]
    public async Task<ActionResult<MemberDto>> GetByMemberNo(string memberNo)
    {
        var member = await _memberService.GetByMemberNoAsync(memberNo);
        if (member == null)
            return NotFound(new { message = "会员不存在" });

        return Ok(member);
    }

    /// <summary>
    /// 创建会员
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberRequest request)
    {
        // 验证手机号是否已存在
        if (await _memberService.ExistsByPhoneAsync(request.Phone))
        {
            return BadRequest(new { message = "该手机号已被注册" });
        }

        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            var createdBy = int.TryParse(userIdClaim, out var uid) ? uid : 0;

            var member = await _memberService.CreateAsync(request, createdBy);
            return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建会员失败");
            return StatusCode(500, new { message = "创建会员失败" });
        }
    }

    /// <summary>
    /// 更新会员
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<MemberDto>> Update(int id, [FromBody] UpdateMemberRequest request)
    {
        var member = await _memberService.UpdateAsync(id, request);
        if (member == null)
            return NotFound(new { message = "会员不存在" });

        return Ok(member);
    }

    /// <summary>
    /// 删除会员（软删除）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _memberService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = "会员不存在" });

        return NoContent();
    }

    /// <summary>
    /// 搜索会员
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<MemberDto>>> Search([FromQuery] string? keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new MemberQueryParams
        {
            Keyword = keyword,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _memberService.GetPagedAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// 获取会员积分
    /// </summary>
    [HttpGet("{id}/points")]
    public async Task<ActionResult<object>> GetPoints(int id)
    {
        var points = await _memberService.GetMemberPointsAsync(id);
        return Ok(new { memberId = id, points });
    }
}
