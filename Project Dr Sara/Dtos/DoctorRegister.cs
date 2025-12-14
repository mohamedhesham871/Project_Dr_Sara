using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Project_Dr_Sara.Dtos
{
    public class DoctorRegister
    {
        public string UserName { get; set; } = string.Empty; //unique
        public string ArabicName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public Specialization? Specialization { get; set; } 
        public string Phone1 { get; set; } =string.Empty;
        public string? Phone2 { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } =string.Empty;
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } =string.Empty;
        public string? Bio { get; set; }
        public string? OpenHour { get; set; }
        public string? CloseHour { get; set; }
        public string? Address { get; set; }

    }
}

