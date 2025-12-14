using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Services.IServices;
using System.Security.Claims;

namespace Project_Dr_Sara.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController(IAppointment service) : ControllerBase
    {
        private readonly IAppointment _service = service;

        [HttpPost("NewAppointment/{DoctorId}")]
        public async Task<IActionResult> GetAppointments(CreateAppointmentRequest request, [FromRoute] string DoctorId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _service.GenerateNewAppointment(request, userId, DoctorId);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }

        [HttpPatch("UpdateStatus/{AppointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(UpdateStatusDto updateStatus, [FromRoute] string AppointmentId)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.UpdateAppointmentStatus(updateStatus, AppointmentId);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteAppointment/{AppointmentId}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] string AppointmentId)
        {
            var result = await _service.DeleteAppointment(AppointmentId);
            return Ok(result);
        }
        [HttpGet("PatientAppointments")]
        public async Task<IActionResult> GetPatientAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetPatientAppointments(userId);
            return Ok(result);
        }
        [HttpGet("PatientPendingAppointments")]
        public async Task<IActionResult> GetPatientPendingAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetPatientPendingAppointments(userId);
            return Ok(result);
        }
        [HttpGet("DoctorAppointments")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetDoctorAppointments(userId);
            return Ok(result);
        }
        [HttpGet("AppointmentDetails/{AppointmentId}")]
        public async Task<IActionResult> GetAppointmentDetails([FromRoute] string AppointmentId)
        {
            var result = await _service.GetAppointmentDetails(AppointmentId);
            return Ok(result);
        }
    }

}