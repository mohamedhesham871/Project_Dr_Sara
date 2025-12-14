using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Repo.IRepo;
using Project_Dr_Sara.Services.IServices;

namespace Project_Dr_Sara.Services
{
    public class SpecializationServices (ISpecilizationRepo repo): Ispecializtion
    {
        public async Task<IList<specilzationsDto>> GetAllSpecializations()
        {
            var res= await repo.GetAllSpecializations();
            return   res.Select(s => new specilzationsDto
            {
                Id = s.Id.ToString(),
                Name = s.Name,
            }).ToList();
        }
    }
}
