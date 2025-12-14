using Project_Dr_Sara.Models;

namespace Project_Dr_Sara.Dtos
{
    public class CreateAppointmentRequest
    {
        public string PatientName { get; set;} = string.Empty;
        public string? Description { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
    }
}