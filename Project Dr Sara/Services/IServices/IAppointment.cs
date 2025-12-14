using Project_Dr_Sara.Dtos;
using System.Data;

namespace Project_Dr_Sara.Services.IServices
{
    public interface IAppointment
    {
        Task<GenericResponseDto> GenerateNewAppointment(CreateAppointmentRequest request, string PatientUserId, string DoctorUserId);
        Task<GenericResponseDto> UpdateAppointmentStatus(UpdateStatusDto updateStatus,string Appointmentid);
        Task<GenericResponseDto> DeleteAppointment(string appointmentId);
        Task<IEnumerable<ShortAppointmentDto>> GetPatientAppointments(string patientId);
        Task<IEnumerable<ShortAppointmentDto>> GetPatientPendingAppointments(string patientId);
        Task<IEnumerable<ShortAppointmentDto>> GetDoctorAppointments(string doctorId);
        Task<AppointmentDetails> GetAppointmentDetails(string Appointment);




    }
}
