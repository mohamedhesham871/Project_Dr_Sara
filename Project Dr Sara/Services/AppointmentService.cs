using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Excepions;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;
using Project_Dr_Sara.Services.IServices;

namespace Project_Dr_Sara.Services
{
    public class AppointmentService(ILogger<AppointmentService> logger,IAppointmentRepo repo ,IDoctorRepo doctorRepo,IPatientRepo patientRepo) : IAppointment
    {
        private readonly ILogger<AppointmentService> _logger = logger;
        private readonly IAppointmentRepo r_epo = repo;
        private readonly IDoctorRepo _doctorRepo = doctorRepo;
        private readonly IPatientRepo _patientRepo = patientRepo;

        public async Task<GenericResponseDto> DeleteAppointment(string appointmentId)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete appointment with ID: {appointmentId}");
                if(appointmentId == null)
                {
                    _logger.LogWarning("Appointment ID is null.");
                    throw new BadRequestException("Appointment ID cannot be null."); 
                }
                var AppointmentDeleted = await r_epo.GetAppointmentDetails(appointmentId);
                if (AppointmentDeleted != null)
                {
                    _logger.LogWarning("Appointment not found.");
                    throw new BadRequestException("Appointment not found.");
                }
                var result = r_epo.DeleteAppointmentDelete(AppointmentDeleted);
                _logger.LogInformation($"Appointment with ID: {appointmentId} deleted successfully.");
                return new GenericResponseDto
                {
                    Success = true,
                    Message = "Appointment deleted successfully.",
                    StatusCode = 200,
                    Errors = null,
                    Timestamp = DateTime.UtcNow
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(DeleteAppointment)}: {ex.Message}");
                return (new GenericResponseDto
                {
                    Success = false,
                    Message = "An error occurred while deleting the appointment.",
                    StatusCode = 500,
                    Errors = new List<string> { ex.Message },
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public async Task<GenericResponseDto> GenerateNewAppointment(CreateAppointmentRequest request,string PatientUserId ,string DoctorUserId)
        {
            try
            {
                _logger.LogInformation("Attempting to generate a new appointment.");
                if(request == null)
                {
                    _logger.LogWarning("CreateAppointmentRequest is null.");
                    throw new BadRequestException("Request data cannot be null."); 
                }
                var patient = await _patientRepo.GetPatientByUserIdAsync(PatientUserId);
                var doctor = await _doctorRepo.GetdoctorByUserIdAsync(DoctorUserId);
                if (patient == null || doctor == null)
                {
                    _logger.LogWarning("Patient or Doctor not found.");
                    throw new BadRequestException("Invalid Patient or Doctor information.");
                }

                var createAppointment = new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = doctor.Id,
                    PatientName = request.PatientName,
                    DoctorName = doctor.EnglishName,
                    Description = request.Description,
                    Phone = request.Phone,
                    AppointmentDate = request.AppointmentDate,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                 await r_epo.GenerateNewAppointment(createAppointment);
                _logger.LogInformation("New appointment generated successfully.");
                return new GenericResponseDto
                {
                    Success = true,
                    Message = "Appointment created successfully.",
                    StatusCode = 201,
                    Errors = null,
                    Timestamp = DateTime.UtcNow
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GenerateNewAppointment)}: {ex.Message}");
                return (new GenericResponseDto
                {
                    Success = false,
                    Message = "An error occurred while generating the appointment.",
                    StatusCode = 500,
                    Errors = new List<string> { ex.Message },
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public async  Task<AppointmentDetails> GetAppointmentDetails(string appointment)
        {
            try
            {
                _logger.LogInformation($"Attempting to retrieve appointment details for ID: {appointment}");
                if(appointment == null)
                {
                    _logger.LogWarning("Appointment ID is null.");
                    throw new BadRequestException("Appointment ID cannot be null."); 
                }
                var appointmentDetails = await r_epo.GetAppointmentDetails(appointment);
                if (appointmentDetails == null)
                {
                    _logger.LogWarning("Appointment not found.");
                    throw new BadRequestException("Appointment not found.");
                }
                _logger.LogInformation($"Appointment details for ID: {appointment} retrieved successfully.");
                return new AppointmentDetails(){
                   
                    AppointmentDate = appointmentDetails.AppointmentDate,
                    Description = appointmentDetails.Description,
                    DoctorName = appointmentDetails.DoctorName,
                    Id = appointmentDetails.Id,
                    PatientName = appointmentDetails.PatientName,
                    Phone = appointmentDetails.Phone,
                    Status = appointmentDetails.Status
                } ;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetAppointmentDetails)}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the appointment details.", ex);
            }
        }

        public async Task<IEnumerable<ShortAppointmentDto>> GetDoctorAppointments(string doctorId)
        {
            try
            {
                _logger.LogInformation($"Attempting to retrieve appointments for doctor ID: {doctorId}");
                if(doctorId == null)
                {
                    _logger.LogWarning("Doctor ID is null.");
                    throw new BadRequestException("Doctor ID cannot be null.");
                }

                var appointments = await  r_epo.GetDoctorAppointments(doctorId);
                if (appointments == null)
                {
                    _logger.LogWarning("No appointments found for the specified doctor.");
                    throw new BadRequestException("No appointments found for the specified doctor.");
                }
                _logger.LogInformation($"Appointments for doctor ID: {doctorId} retrieved successfully.");
                return new List<ShortAppointmentDto>(appointments.Select(a => new ShortAppointmentDto
                {
                    Id = a.Id,
                    PatientName = a.PatientName,
                    DoctorName = a.DoctorName,
                    AppointmentDate = a.AppointmentDate
                }));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetDoctorAppointments)}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the doctor's appointments.", ex);
            }
        }

        public async Task<IEnumerable<ShortAppointmentDto>> GetPatientAppointments(string patientId)
        {
            try
            {
                _logger.LogInformation($"Attempting to retrieve appointments for patient ID: {patientId}");
                if(patientId == null)
                {
                    _logger.LogWarning("Patient ID is null.");
                    throw new BadRequestException("Patient ID cannot be null.");
                }
                var appointments =  await r_epo.GetPatientAppointments(patientId);
                if (appointments == null)
                {
                    _logger.LogWarning("No appointments found for the specified patient.");
                    throw new BadRequestException("No appointments found for the specified patient.");
                }
                _logger.LogInformation($"Appointments for patient ID: {patientId} retrieved successfully.");
                return (new List<ShortAppointmentDto>(appointments.Select(a => new ShortAppointmentDto
                {
                    Id = a.Id,
                    PatientName = a.PatientName,
                    DoctorName = a.DoctorName,
                    AppointmentDate = a.AppointmentDate
                })));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetPatientAppointments)}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the patient's appointments.", ex);
            }
        }

        public async Task<IEnumerable<ShortAppointmentDto>> GetPatientPendingAppointments(string patientId)
        {
            try
            {
                _logger.LogInformation($"Attempting to retrieve pending appointments for patient ID: {patientId}");
                if (patientId == null)
                {
                    _logger.LogWarning("Patient ID is null.");
                    throw new BadRequestException("Patient ID cannot be null.");
                }
                var appointments = await r_epo.GetPatientPendingAppointments(patientId);
                if (appointments == null)
                {
                    _logger.LogWarning("No pending appointments found for the specified patient.");
                    throw new BadRequestException("No pending appointments found for the specified patient.");
                }
                _logger.LogInformation($"Pending appointments for patient ID: {patientId} retrieved successfully.");
                return (new List<ShortAppointmentDto>(appointments.Select(a => new ShortAppointmentDto
                {
                    Id = a.Id,
                    PatientName = a.PatientName,
                    DoctorName = a.DoctorName,
                    AppointmentDate = a.AppointmentDate
                })));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetPatientPendingAppointments)}: {ex.Message}");
                throw new Exception("An error occurred while retrieving the patient's pending appointments.", ex);
            }
        }

        public async Task<GenericResponseDto> UpdateAppointmentStatus(UpdateStatusDto updateStatus, string AppointmentId)
        {
            try
            {
                _logger.LogInformation($"Attempting to update appointment status for ID: {updateStatus}");
                if (updateStatus == null)
                {
                    _logger.LogWarning("UpdateStatusDto is null.");
                    throw new BadRequestException("Update status data cannot be null.");
                }
                if (AppointmentId == null)
                {
                    _logger.LogWarning("Appointment ID is null.");
                    throw new BadRequestException("Appointment ID cannot be null.");
                }
                var appointmentToUpdate = await r_epo.GetAppointmentDetails(AppointmentId);
                appointmentToUpdate.Status = updateStatus.Status.ToString();
                if (appointmentToUpdate == null)
                {
                    _logger.LogWarning("Appointment not found.");
                    throw new BadRequestException("Appointment not found.");
                }
                await r_epo.UpdateAppointmentStatus(appointmentToUpdate);

                _logger.LogInformation($"Appointment status for ID: {AppointmentId} updated successfully.");
                return (new GenericResponseDto
                {
                    Success = true,
                    Message = "Appointment status updated successfully.",
                    StatusCode = 200,
                    Errors = null,
                    Timestamp = DateTime.UtcNow
                });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(UpdateAppointmentStatus)}: {ex.Message}");
                return new GenericResponseDto
                {
                    Success = false,
                    Message = "An error occurred while updating the appointment status.",
                    StatusCode = 500,
                    Errors = new List<string> { ex.Message },
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }
}
