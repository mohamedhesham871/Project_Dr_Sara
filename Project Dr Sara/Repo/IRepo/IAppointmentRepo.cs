using Project_Dr_Sara.Models;

namespace Project_Dr_Sara.Repo.IRepo
{
    public interface IAppointmentRepo
    {
        Task GenerateNewAppointment(Appointment  appointment);
        Task UpdateAppointmentStatus(Appointment updateStatus);
        Task DeleteAppointmentDelete(Appointment Deleted);
        Task <List<Appointment>>  GetPatientAppointments(string patientId);
        Task <List<Appointment>> GetPatientPendingAppointments(string patientId);
        Task <List<Appointment>> GetDoctorAppointments(string doctorId);
        Task<Appointment> GetAppointmentDetails(string Appointment);
    }
}
        