namespace Project_Dr_Sara.Dtos
{
    public class UserResponse
    {
        public string Name { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
