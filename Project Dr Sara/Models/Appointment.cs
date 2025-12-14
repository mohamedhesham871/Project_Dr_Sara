namespace Project_Dr_Sara.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }//Fk for Patient
        public Guid DoctorId { get; set; }//Fk for Doctor
        public string PatientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public bool IsComplete => Status == "Completed";
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        // Navigation properties
        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }

        public Appointment()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}


