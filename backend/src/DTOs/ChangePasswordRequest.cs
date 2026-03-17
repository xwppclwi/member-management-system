using System.ComponentModel.DataAnnotations;

namespace MemberManagement.DTOs;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "旧密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "新密码长度至少为6位")]
    public string NewPassword { get; set; } = string.Empty;
}
