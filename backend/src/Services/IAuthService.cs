using MemberManagement.DTOs;

namespace MemberManagement.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request, int? currentUserId = null);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
}
