using BCrypt.Net;
using MemberManagement.Common;
using MemberManagement.DTOs;
using MemberManagement.Models;
using MemberManagement.Repositories;

namespace MemberManagement.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null)
        {
            return new AuthResponse { Success = false, Message = "用户名或密码错误" };
        }

        if (!user.IsActive)
        {
            return new AuthResponse { Success = false, Message = "账号已被禁用" };
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponse { Success = false, Message = "用户名或密码错误" };
        }

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Success = true,
            Message = "登录成功",
            Token = token,
            User = MapToDto(user)
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, int? currentUserId = null)
    {
        // 检查是否有权限创建用户（只有Admin可以创建其他Admin账号）
        if (request.Role == "Admin" && currentUserId.HasValue)
        {
            var currentUser = await _userRepository.GetByIdAsync(currentUserId.Value);
            if (currentUser?.Role != "Admin")
            {
                return new AuthResponse { Success = false, Message = "只有管理员可以创建管理员账号" };
            }
        }

        if (await _userRepository.ExistsAsync(request.Username))
        {
            return new AuthResponse { Success = false, Message = "用户名已存在" };
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "注册成功",
            User = MapToDto(user)
        };
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);

        return true;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
