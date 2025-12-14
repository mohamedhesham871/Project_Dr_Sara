using Project_Dr_Sara.Dtos;

namespace Project_Dr_Sara.Services.IServices
{
    public interface Ispecializtion
    {
        Task<IList<specilzationsDto>> GetAllSpecializations();
    }
}
