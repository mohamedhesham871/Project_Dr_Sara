using System.ComponentModel.DataAnnotations;

namespace Project_Dr_Sara.Dtos
{
    public class UserLogInRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } =string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }=string.Empty;
    }
}
