using Project_Dr_Sara.Models;

namespace Project_Dr_Sara.Repo.IRepo
{
    public interface ISpecilizationRepo
    {
        Task<IList<Specializations>> GetAllSpecializations();
    } 
}
