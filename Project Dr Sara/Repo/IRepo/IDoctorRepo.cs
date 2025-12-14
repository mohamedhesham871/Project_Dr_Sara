using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Models;
using System;

namespace Project_Dr_Sara.Repo.IRepo
{
    public interface IDoctorRepo
    {
        
            Task<IEnumerable<Doctor>> GetAllAsync();
            Task<IEnumerable<Doctor>> GetAllDoctorsInSpecializationAsync(string specilization);
            Task<IEnumerable<Doctor>> GetAllDoctorsInSpecializationTopRatedAsync(string specilization);
            Task<IEnumerable<Doctor>> GetAllDoctorsInTopRatedAsync();  
           Task<Doctor>GetdoctorByUserIdAsync(string userId);
           void Update(Doctor entity);


    }
}

