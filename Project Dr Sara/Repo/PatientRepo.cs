using Microsoft.EntityFrameworkCore;
using Project_Dr_Sara.DbContexts;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;

namespace Project_Dr_Sara.Repo
{
    public class PatientRepo(AppDbContexts contexts) : IPatientRepo
    {
        private readonly AppDbContexts _contexts = contexts;

        public async Task<Patient?> GetPatientByIdAsync(string id)
        {
            return await _contexts.Patients.Include(p => p.User)
                                           .FirstOrDefaultAsync(p=>p.Id.ToString()==id);
        }
        public async Task<Patient?> GetPatientByUserIdAsync(string id)
        {
            return await _contexts.Patients.Include(p=>p.User)
                                           .FirstOrDefaultAsync(p => p.UserId.ToString() == id);
        }

        public void UpdatePatientProfileAsync(Patient patient)
        {
            _contexts.Patients.Update(patient);
            _contexts.SaveChanges();
        }
    }
}
