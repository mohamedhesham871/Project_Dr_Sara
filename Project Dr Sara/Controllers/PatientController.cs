using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Dr_Sara.Services.IServices;
using System.Security.Claims;

namespace Project_Dr_Sara.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController(IPatientService service):ControllerBase
    {
        private readonly IPatientService _service = service;

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAllPatients()
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res =await  _service.GetPatientDetailsByUserIdAsync( Id);
            return Ok(res);
        }
    }
}
