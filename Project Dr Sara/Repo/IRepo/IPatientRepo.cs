using Project_Dr_Sara.Models;

namespace Project_Dr_Sara.Repo.IRepo
{
    public interface IPatientRepo
    {
        Task<Patient?> GetPatientByIdAsync(string id);
        Task<Patient?> GetPatientByUserIdAsync(string id);

        void UpdatePatientProfileAsync(Patient patient);
    }
}