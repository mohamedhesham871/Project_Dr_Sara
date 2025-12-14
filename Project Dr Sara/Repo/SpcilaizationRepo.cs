using Microsoft.EntityFrameworkCore;
using Project_Dr_Sara.DbContexts;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;

namespace Project_Dr_Sara.Repo
{
    public class SpcilaizationRepo(AppDbContexts contexts ) : ISpecilizationRepo
    {
        public async Task<IList<Specializations>> GetAllSpecializations()
        {
            return await contexts.Specializations.ToListAsync();
        }
    }
}
