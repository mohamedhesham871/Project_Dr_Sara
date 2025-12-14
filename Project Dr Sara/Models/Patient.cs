namespace Project_Dr_Sara.Models
{
    public class Patient
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string ArabicName { get; set; } = null!;
        public string EnglishName { get; set; } = null!;
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; } // Age is calculated from BirthDate
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public Patient() { }

        // Navigation property
        public User? User { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}