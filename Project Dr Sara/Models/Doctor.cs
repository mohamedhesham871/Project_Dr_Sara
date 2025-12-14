namespace Project_Dr_Sara.Models
{
    public class Doctor
    {
        public Guid UserId { get; set; } //Fk
        public Guid Id { get; set; }
        public string ArbicName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string PhoneNumber1 { get; set; } = string.Empty;
        public string? PhoneNumber2 { get; set; }
        public string ?Bio {  get; set; }
        public string? OpenHour { get; set; }
        public string? CloseHour { get; set; }
        public string Address { set; get; } = string.Empty;
        public string specialization { get; set; } = string.Empty;
        public User? User { get; set; } //Nav Prop

        public Guid? SpecializationNavId { get; set; } //Fk
        public Specializations? specializationNav { get; set; } //Nav Prop
    }
}
