namespace Project_Dr_Sara.Dtos
{
    public class ShortAppointmentDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
    }
}
