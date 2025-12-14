using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Services.IServices;
using System.Security.Claims;

namespace Project_Dr_Sara.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationUserController(IAuthService service) : ControllerBase
    {
        [HttpPost("register/Patient")]
        public async Task<IActionResult> RegisterPatient([FromForm] PatientRegister user)
        {

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var result = await service.RegisterPatient(user);
            return Ok(result);
        }

        [HttpPost("register/Doctor")]
        public async Task<IActionResult> RegisterDoctor([FromForm] DoctorRegister user)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var result = await service.RegisterDoctor(user);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogInRequest user)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var result = await service.Login(user);
            return Ok(result);
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var result = await service.ChangePassword(dto, token);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("resetPassword")]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await service.ResetPassword(dto);
            return Ok(result);
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await service.ForgetPassword(dto);
            return Ok(result);
        }

        [HttpPut("updateDoctorProfile")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateDoctorProfile([FromForm] UpateDoctorDto doctorUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await service.UpdateDoctorProfileAsync(userId!, doctorUpdateDto);
            return Ok(result);
        }

        [HttpPut("UpatePatientProfile")]
        [Authorize(Roles ="Patient")]
        public async Task<IActionResult> UpdatePatientProfile([FromForm] UpdatePatientDto updatePatientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await service.UpdatePatientDetailsAsync(userId!, updatePatientDto);
            return Ok(result);
        }

        [HttpPatch("UpdateImageProfile")]
        [Authorize]
        public async Task<IActionResult> ChangeIamgeProfile([FromForm]UpdateProfileimage updateProfileimage)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var useRole = User.FindFirstValue(ClaimTypes.Role);
            var res = await service.UpdateProfileImage(updateProfileimage, userId, useRole);    
            return Ok(res);
        }


    }

}
