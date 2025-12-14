using Microsoft.AspNetCore.Mvc;
using Project_Dr_Sara.Services.IServices;

namespace Project_Dr_Sara.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeContollercs(Ispecializtion service):ControllerBase
    {
        private readonly Ispecializtion service = service;

        [HttpGet("Welcome")]
        
        public async Task<IActionResult> Welcome()
        {
            var res= await service.GetAllSpecializations();
            return Ok(res);
        }
    }
}
