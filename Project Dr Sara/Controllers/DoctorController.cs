using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Services;
using Project_Dr_Sara.Services.IServices;
using System.Security.Claims;

namespace Project_Dr_Sara.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController(IDoctorService service) : ControllerBase
    {
        private readonly IDoctorService _service = service;

        [HttpGet("{SpecializationId}")]
        public async Task<IActionResult> GetAllDoctorsInSpecializationAsync([FromRoute]string SpecializationId)
        {
            if (!ModelState.IsValid)
                return BadRequest(SpecializationId);

            var res= _service.GetAllDoctorsInSpecializationAsync(SpecializationId);
            return Ok(res);

        }

        [HttpGet("AllDoctor")]
        public async Task<IActionResult> GetAllDoctorsAsync()
        { 
           var res= _service.GetAllDoctorsAsync();
            return Ok(res);
        }


        [HttpGet("{SpecializationId}/Top_Rate")]
        public async Task<IActionResult> GetAllDoctorsInSpecializationTopRatedAsync([FromRoute]string SpecializationId)

        {
            if (!ModelState.IsValid)
                return BadRequest(SpecializationId);

            var res = _service.GetAllDoctorsInSpecializationTopRatedAsync(SpecializationId);
            return Ok(res);

        }
        [HttpGet("Top_Rate")]
        public async Task<IActionResult> GetAllDoctorsInTopRatedAsync()
        {
            var res = _service.GetAllDoctorsInTopRatedAsync();
            return Ok(res);

        }

        [HttpGet]
        [Authorize(Roles ="Doctor")]
        public async Task<IActionResult> GetDoctorByIdAsync()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rse= _service.GetDoctorByIdAsync(user);
            return Ok(rse);

        }


    }
}
