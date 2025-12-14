using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Excepions;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;
using Project_Dr_Sara.Services.IServices;

namespace Project_Dr_Sara.Services
{
    public class PatientService(ILogger<PatientService> logger,IPatientRepo repo) : IPatientService
    {
        private readonly ILogger<PatientService> _logger = logger;
        private readonly IPatientRepo _repo = repo;

        public async Task<PatientDetailsDto> GetPatientDetailsByUserIdAsync(string userId)
        {
            try
            {
                //check On Patient Id
                _logger.LogInformation("Getting patient details for user ID: {userId}", userId);
                var patientDetails = await _repo.GetPatientByUserIdAsync(userId);
                if (patientDetails == null)
                    throw new NotFoundException($"Patient with ID {userId} not found.");

                var patientDetailsDto = new PatientDetailsDto
                {
                    UserId = patientDetails.UserId.ToString(),
                    Email = patientDetails.User?.Email,
                    profileImage = patientDetails.User?.ImageProfile,
                    BirthDate = patientDetails.BirthDate,
                    Age = patientDetails.Age,
                    Phone = patientDetails.Phone,
                    Bio = patientDetails.Bio,
                    City = patientDetails.City
                };
                _logger.LogInformation("Successfully retrieved patient details for User ID: {userId}", userId);
                return patientDetailsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting patient details by ID: {userId}", userId);
                throw;
            }
        }
    }
}
