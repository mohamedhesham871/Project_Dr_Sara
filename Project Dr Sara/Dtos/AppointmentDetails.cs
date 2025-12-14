namespace Project_Dr_Sara.Dtos
{
    public class AppointmentDetails
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public bool IsComplete => Status == "Completed";
        public string Status { get; set; } = "Pending";
    }
}
