using Microsoft.EntityFrameworkCore;
using Project_Dr_Sara.DbContexts;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;

namespace Project_Dr_Sara.Repo
{
    public class DoctorRepo (AppDbContexts contexts): IDoctorRepo
    {
        private readonly AppDbContexts _contexts = contexts;

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _contexts.Doctors.Include(u=>u.User).ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsInSpecializationAsync(string Specialization)
        {
           return await _contexts.Doctors.Include(u => u.User).Where(d=>d.specialization==Specialization).ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsInSpecializationTopRatedAsync(string Specialization)
        {
            return await _contexts.Doctors.Include(u => u.User).Where(d => d.specialization == Specialization).OrderByDescending(d=>d.Rate).ToListAsync();
        }
        public async Task<IEnumerable<Doctor>> GetAllDoctorsInTopRatedAsync()
        {
            return await _contexts.Doctors.Include(u => u.User).OrderByDescending(d => d.Rate).ToListAsync();

        }

       

        public Task<Doctor> GetdoctorByUserIdAsync(string userId)
        {
            return _contexts.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId.ToString() == userId);
        }

        public void Update(Doctor entity)
        {
              _contexts.Doctors.Update(entity);
            _contexts.SaveChanges();
        }
    }
}
