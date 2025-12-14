using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Excepions;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;
using Project_Dr_Sara.Services.IServices;

namespace Project_Dr_Sara.Services
{
    public class DoctorService(IDoctorRepo repo,UserManager<User> userManager ,ILogger<DoctorService>Logger) : IDoctorService
    {
        private readonly IDoctorRepo _repo = repo;
        private readonly UserManager<User> userManager = userManager;
        private readonly ILogger<DoctorService> _logger = Logger;

        public async  Task<IEnumerable<DoctorShortDto>> GetAllDoctorsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all doctors from the repository.");
                var doctors = await _repo.GetAllAsync();
                var doctorShortDtos = doctors.Select(doctor => new DoctorShortDto
                {
                    UsreId = doctor.UserId.ToString(),
                    ArabicName = doctor.ArbicName,
                    EnglishName = doctor.EnglishName,
                    Specialization = doctor.specialization,
                    Rating = doctor.Rate,
                    ProfilePictureUrl = doctor.User!.ImageProfile!
                });
                _logger.LogInformation("Successfully fetched and mapped all doctors.");
                return doctorShortDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all doctors.");
                throw;
            }
        }

        public async Task<IEnumerable<DoctorShortDto>> GetAllDoctorsInSpecializationAsync(string SpecializationId)
        {
            try
            {
                _logger.LogInformation("Fetching doctors in specialization {SpecializationId} from the repository.", SpecializationId);
        
                if(string.IsNullOrEmpty(SpecializationId))
                {
                    _logger.LogWarning("SpecializationId is null or empty.");
                    throw new ArgumentException("SpecializationId cannot be null or empty.", nameof(SpecializationId));
                }
                var doctors = _repo.GetAllDoctorsInSpecializationAsync(SpecializationId);
                var doctorShortDtos = doctors.Result.Select(doctor => new DoctorShortDto
                {
                    UsreId = doctor.UserId.ToString(),
                    ArabicName = doctor.ArbicName,
                    EnglishName = doctor.EnglishName,
                    Specialization = doctor.specialization,
                    Rating = doctor.Rate,
                    ProfilePictureUrl = doctor.User!.ImageProfile!
                });
                _logger.LogInformation("Successfully fetched and mapped doctors in specialization {SpecializationId}.", SpecializationId);
                return (doctorShortDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching doctors in specialization {SpecializationId}.", SpecializationId);
                throw;
            }
        }

        public async Task<IEnumerable<DoctorShortDto>> GetAllDoctorsInSpecializationTopRatedAsync(string SpecializationId)
        {
            try { 
                _logger.LogInformation("Fetching top-rated doctors in specialization {SpecializationId} from the repository.", SpecializationId);
        
                if(string.IsNullOrEmpty(SpecializationId))
                {
                    _logger.LogWarning("SpecializationId is null or empty.");
                    throw new ArgumentException("SpecializationId cannot be null or empty.", nameof(SpecializationId));
                }
                var doctors = _repo.GetAllDoctorsInSpecializationTopRatedAsync(SpecializationId);
                var doctorShortDtos = doctors.Result.Select(doctor => new DoctorShortDto
                {
                    UsreId = doctor.UserId.ToString(),
                    ArabicName = doctor.ArbicName,
                    EnglishName = doctor.EnglishName,
                    Specialization = doctor.specialization,
                    Rating = doctor.Rate,
                    ProfilePictureUrl = doctor.User!.ImageProfile!
                });
                _logger.LogInformation("Successfully fetched and mapped top-rated doctors in specialization {SpecializationId}.", SpecializationId);
                return (doctorShortDtos);
            }
            catch (Exception ex) { throw; }

        }

        public async Task<IEnumerable<DoctorShortDto>> GetAllDoctorsInTopRatedAsync()
        {
            _logger.LogInformation("Fetching all doctors from the repository.");
            var doctors = await _repo.GetAllDoctorsInTopRatedAsync();
            var doctorShortDtos = doctors.Select(doctor => new DoctorShortDto
            {
                UsreId = doctor.UserId.ToString(),
                ArabicName = doctor.ArbicName,
                EnglishName = doctor.EnglishName,
                Specialization = doctor.specialization,
                Rating = doctor.Rate,
                ProfilePictureUrl = doctor.User!.ImageProfile!
            });
            _logger.LogInformation("Successfully fetched and mapped all doctors.");
            return doctorShortDtos;
        }

        public async Task<DoctorDetails> GetDoctorByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Fetching doctor by ID {DoctorId} from the repository.", id);

                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Doctor ID is null or empty.");
                    throw new ArgumentException("Doctor ID cannot be null or empty.", nameof(id));
                }
                var doctor = await _repo.GetdoctorByUserIdAsync(id);
                if (doctor == null)
                {
                    _logger.LogWarning("Doctor with ID {DoctorId} not found.", id);
                    throw new KeyNotFoundException($"Doctor with ID {id} not found.");
                }
                var doctorDetailsDto = new DoctorDetails
                {
                    UserId = doctor.UserId.ToString(),
                    arabicName = doctor.ArbicName,
                    EnglishName = doctor.EnglishName,
                    Specialization = doctor.specialization,
                    Rating = doctor.Rate,
                    ProfilePictureUrl = doctor.User!.ImageProfile!,
                    Bio = doctor.Bio,
                    Phone1 = doctor.PhoneNumber1,
                    Phone2 = doctor.PhoneNumber2,
                    openingHours = doctor.OpenHour,
                    closedHours = doctor.CloseHour,
                    Address = doctor.Address
                };
                _logger.LogInformation("Successfully fetched and mapped doctor by ID {DoctorId}.", id);
                return doctorDetailsDto;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching doctor by ID {DoctorId}.", id);
                throw;
            }
        }

      


    }
}
