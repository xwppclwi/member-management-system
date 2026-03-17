using System.Security.Claims;
using MemberManagement.DTOs;
using MemberManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemberManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { code = 400, message = "参数验证失败", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        var response = await _authService.LoginAsync(request);

        if (!response.Success)
        {
            return Unauthorized(new { code = 401, message = response.Message });
        }

        return Ok(new { code = 200, data = new { token = response.Token, user = response.User }, message = response.Message });
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { code = 400, message = "参数验证失败", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        var currentUserId = GetCurrentUserId();
        var response = await _authService.RegisterAsync(request, currentUserId);

        if (!response.Success)
        {
            return BadRequest(new { code = 400, message = response.Message });
        }

        return Ok(new { code = 200, data = response.User, message = response.Message });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new { code = 401, message = "无效的Token" });
        }

        var user = await _authService.GetCurrentUserAsync(userId.Value);
        if (user == null)
        {
            return NotFound(new { code = 404, message = "用户不存在" });
        }

        return Ok(new { code = 200, data = user, message = "获取成功" });
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { code = 400, message = "参数验证失败", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new { code = 401, message = "无效的Token" });
        }

        var success = await _authService.ChangePasswordAsync(userId.Value, request);
        if (!success)
        {
            return BadRequest(new { code = 400, message = "旧密码错误" });
        }

        return Ok(new { code = 200, message = "密码修改成功" });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
