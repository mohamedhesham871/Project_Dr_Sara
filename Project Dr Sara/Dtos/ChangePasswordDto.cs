using System.ComponentModel.DataAnnotations;

namespace Project_Dr_Sara.Dtos
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [RegularExpression(
             @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number, and one special character"
         )]
        
        public string NewPassword { get; set; } = string.Empty;
    }
}
