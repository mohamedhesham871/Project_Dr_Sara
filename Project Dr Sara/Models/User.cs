using Microsoft.AspNetCore.Identity;

namespace Project_Dr_Sara.Models
{
    public class User:IdentityUser
    {
        public string? ImageProfile { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public User()
        {
            CreateAt = DateTime.UtcNow;
            UpdateAt = DateTime.UtcNow;
        }
    }
}

