namespace Project_Dr_Sara.Dtos
{
    public class PatientDetailsDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public string profileImage { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; } 
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
    }
}
