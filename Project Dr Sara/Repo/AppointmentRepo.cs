using Microsoft.EntityFrameworkCore;
using Project_Dr_Sara.DbContexts;
using Project_Dr_Sara.Excepions;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;

namespace Project_Dr_Sara.Repo
{
    public class AppointmentRepo(AppDbContexts contexts) : IAppointmentRepo
    {
        private readonly AppDbContexts _contexts = contexts;

        //Crate New Appointment
        public async Task GenerateNewAppointment(Appointment appointment)
        {
            await _contexts.Appointments.AddAsync(appointment);
            await _contexts.SaveChangesAsync();
        }
        //Delete Appointment
        public async Task DeleteAppointmentDelete(Appointment appointment)
        {
            _contexts.Appointments.Remove(appointment);
            await _contexts.SaveChangesAsync();

        }
        //Update Appointment
        public async Task UpdateAppointmentStatus(Appointment updateStatus)
        {
            _contexts.Appointments.Update(updateStatus);
            await _contexts.SaveChangesAsync();
        }

        // Get Appointment by Id
        public async Task<Appointment> GetAppointmentDetails(string AppointmentId)
        {
            return await _contexts.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id.ToString() == AppointmentId);
        }

        public async Task<List<Appointment>> GetDoctorAppointments(string doctorId)
        {
            if (!Guid.TryParse(doctorId, out var doctorGuid))
                throw new BadRequestException("Invalid doctor ID format.");

            var appointments = await _contexts.Appointments
                .Where(a => a.DoctorId == doctorGuid && a.Status != "Cancelled")
                .Include(a => a.Patient)
                .ToListAsync();
            return appointments;
        }

        public async Task<List<Appointment>> GetPatientAppointments(string patientId)
        {
            if (!Guid.TryParse(patientId, out var patientGuid))
                throw new BadRequestException("Invalid patient ID format.");

            return await _contexts.Appointments
                .Where(a => a.PatientId == patientGuid && a.Status != "Cancelled")
                .Include(a => a.Doctor)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetPatientPendingAppointments(string patientId)
        {
            if (!Guid.TryParse(patientId, out var patientGuid))
                throw new BadRequestException("Invalid patient ID format.");

            return await _contexts.Appointments
                .Where(a => a.PatientId == patientGuid && a.Status == "Pending")
                .Include(a => a.Doctor)
                .ToListAsync();
        }


    }
}
