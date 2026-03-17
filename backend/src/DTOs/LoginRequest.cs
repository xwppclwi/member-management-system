using System.ComponentModel.DataAnnotations;

namespace MemberManagement.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50之间")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度至少为6位")]
    public string Password { get; set; } = string.Empty;
}
