using Project_Dr_Sara.Dtos;

namespace Project_Dr_Sara.Services.IServices
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorShortDto>> GetAllDoctorsInSpecializationAsync(string SpecializationId); //Doctors in Specialization
        Task<IEnumerable<DoctorShortDto>> GetAllDoctorsInSpecializationTopRatedAsync(string SpecializationId); //Top rated Doctors in Specialization
        Task<IEnumerable<DoctorShortDto>> GetAllDoctorsInTopRatedAsync(); //All Top rated Doctors
        Task<IEnumerable<DoctorShortDto>> GetAllDoctorsAsync(); //All Doctors
        Task<DoctorDetails> GetDoctorByIdAsync(string id);
    }
}
