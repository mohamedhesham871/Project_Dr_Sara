using System.ComponentModel.DataAnnotations;

namespace Project_Dr_Sara.Dtos
{
    public class PatientRegister
    {
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        [Required(ErrorMessage ="Email address is Required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is Required") ]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }

    }
}
