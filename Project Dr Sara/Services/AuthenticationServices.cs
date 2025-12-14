using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Project_Dr_Sara.DbContexts;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Excepions;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo.IRepo;
using Project_Dr_Sara.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Project_Dr_Sara.Services
{
    public class AuthenticationServices(UserManager<User> userManager ,
        ILogger<AuthenticationServices> logger,
        IOptions<JWT> jwt ,
        AppDbContexts contexts 
        , IConfiguration config,
        IDoctorRepo doctorRepo,
        IPatientRepo patientRepo) : IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILogger<AuthenticationServices> _logger = logger;
        private readonly IOptions<JWT> _jwt = jwt;
        private readonly AppDbContexts _contexts = contexts;
        private readonly IConfiguration _config = config;
        private readonly IDoctorRepo _doctorRepo = doctorRepo;
        private readonly IPatientRepo patientRepo = patientRepo;

        // Login Method
        public async Task<UserResponse> Login(UserLogInRequest userLoginRequest)
        {
            try
            {
                _logger.LogInformation("User with Email {0} Want to LogIn",userLoginRequest.Email);
                //check on Email Or userName
                var user = await _userManager.FindByEmailAsync(userLoginRequest.Email);
                if (user == null)
                {
                    //if user Enter Username it's Unique
                    user = await _userManager.FindByNameAsync(userLoginRequest.Email);
                }
                if (user is not null)
                {
                    var IsPassword = await _userManager.CheckPasswordAsync(user, userLoginRequest.Password);
                    if (!IsPassword)
                    {
                        throw new BadRequestException("Email/UserName OR Password Is Not Correct !");
                    }

                    var userRole = await _userManager.GetRolesAsync(user);// Not Best Practice 
                    var response = new UserResponse()
                    {
                        Email = user.Email!,
                        Name = user.UserName!,
                        Role = userRole.First(),
                        Token = await GenerateJwtToken(user)
                    };
                    _logger.LogInformation("User with Email {0} Want to Logged In Successfully", userLoginRequest.Email);

                }
                throw new BadRequestException("Email/UserName OR Password Is Not Correct !");
            }
            catch(Exception ex)
            {
                _logger.LogError("Error while Login user ", ex.Message);
                throw;
            }
        }

        // Register Doctor Method
        public async Task<UserResponse> RegisterDoctor(DoctorRegister DoctorRegisterRequest)
        {
            try
            {
                //check on Email or UserName if Already found 
                _logger.LogInformation("new Doctor want to Register ");

                if (await _userManager.FindByEmailAsync(DoctorRegisterRequest.Email) is not null ||
                    await _userManager.FindByNameAsync(DoctorRegisterRequest.UserName) is not null)
                {
                    throw new BadRequestException("User is Already Registered Change Email or userName");
                }
                //check on phone 
                if (!String.IsNullOrEmpty(DoctorRegisterRequest.Phone1)&& !Regex.IsMatch(DoctorRegisterRequest.Phone1, @"^(010|011|012|015)[0-9]{8}$"))
                    throw new BadRequestException($"Phone Number :{DoctorRegisterRequest.Phone1} is Invalid");
                if (!String.IsNullOrEmpty(DoctorRegisterRequest.Phone2)&& !Regex.IsMatch(DoctorRegisterRequest.Phone2, @"^(010|011|012|015)[0-9]{8}$"))
                    throw new BadRequestException($"Phone Number :{DoctorRegisterRequest.Phone2} is Invalid");

                //check on image
                var profilePath = "images/Doctorimages/Default_Icone.png";
                if (DoctorRegisterRequest.Image != null && DoctorRegisterRequest.Image.Length > 0)
                    profilePath = await UploadImageAsync(DoctorRegisterRequest.Image, "DoctorImages");

                var User = new User()
                {
                    UserName = DoctorRegisterRequest.UserName,
                    Email = DoctorRegisterRequest.Email,
                    ImageProfile = profilePath
                };
                var CreateUser = await _userManager.CreateAsync(User, DoctorRegisterRequest.Password);

                if (CreateUser.Succeeded)
                {
                    var res = await _userManager.AddToRoleAsync(User, "Doctor");
                    if (!res.Succeeded)
                    {
                        var errors = res.Errors.Select(e => e.Description);
                        throw new ValidationErrorsExecption(errors);
                    }
                }
                // add Doctor Info
                var Doctor = new Doctor()
                {
                    UserId = Guid.Parse(User.Id),
                    ArbicName = DoctorRegisterRequest.ArabicName,
                    EnglishName = DoctorRegisterRequest.EnglishName,
                    specialization = DoctorRegisterRequest.Specialization.ToString(),
                    PhoneNumber1 = DoctorRegisterRequest.Phone1,
                    PhoneNumber2 = DoctorRegisterRequest.Phone2,
                    Bio = DoctorRegisterRequest.Bio,
                    OpenHour = DoctorRegisterRequest.OpenHour,
                    CloseHour = DoctorRegisterRequest.CloseHour,
                    Address = DoctorRegisterRequest.Address
                };
                // save Doctor to Db
                 _contexts.Doctors.Add(Doctor);

                await _contexts.SaveChangesAsync();
                _logger.LogInformation("Doctor with Email {0} Registered Successfully", DoctorRegisterRequest.Email);
                return new UserResponse()
                {
                    Name = User.UserName!,
                    Email = User.Email!,
                    Role = "Doctor",
                    Token = await GenerateJwtToken(User)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Register Doctor ", ex.Message);
                throw;
            }
        }
        // Register Patient Method
        public async Task<UserResponse> RegisterPatient(PatientRegister PatientRegisterRequest)
        {
            try
            {
                //check on Email or UserName if Already found 
                _logger.LogInformation("new Patient want to Register ");

                if (await _userManager.FindByEmailAsync(PatientRegisterRequest.Email) is not null ||
                    await _userManager.FindByNameAsync(PatientRegisterRequest.UserName) is not null)
                {
                    throw new BadRequestException("User is Already Registered Change Email or userName");
                }
                //check on phone 
                if (!String.IsNullOrEmpty(PatientRegisterRequest.Phone) && !Regex.IsMatch(PatientRegisterRequest.Phone, @"^(010|011|012|015)[0-9]{8}$"))
                    throw new BadRequestException($"Phone Number :{PatientRegisterRequest.Phone} is Invalid");

                //check on image
                var profilePath = "images/patientImages/Default_Icone.png";
                if (PatientRegisterRequest.Image != null && PatientRegisterRequest.Image.Length > 0)
                    profilePath = await UploadImageAsync(PatientRegisterRequest.Image, "patientImages");

                var User = new User()
                {
                    UserName = PatientRegisterRequest.UserName,
                    Email = PatientRegisterRequest.Email,
                    ImageProfile = profilePath
                };
                var CreateUser = await _userManager.CreateAsync(User, PatientRegisterRequest.Password);

                if (CreateUser.Succeeded)
                {
                    var res = await _userManager.AddToRoleAsync(User, "Patient");
                    if (!res.Succeeded)
                    {
                        var errors = res.Errors.Select(e => e.Description);
                        throw new ValidationErrorsExecption(errors);
                    }
                }
                // add Patient Info
                var Patient = new Patient()
                {
                    UserId = Guid.Parse(User.Id),
                    BirthDate = PatientRegisterRequest.DateOfBirth,
                    Phone = PatientRegisterRequest.Phone,
                    Age = DateTime.Now.Year - PatientRegisterRequest.DateOfBirth.Year,
                    Bio = PatientRegisterRequest.Bio,
                    City = PatientRegisterRequest.City
                };
                // save Patient to Db
                _contexts.Patients.Add(Patient);
                
                await _contexts.SaveChangesAsync();
               
                _logger.LogInformation("Patient with Email {0} Registered Successfully", PatientRegisterRequest.Email);
               
                return new UserResponse()
                {
                    Name = User.UserName!,
                    Email = User.Email!,
                    Role = "Patient",
                    Token = await GenerateJwtToken(User)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Register Patient ", ex.Message);
                throw;
            }

        }

        // Change Password Method
        public async  Task<GenericResponseDto> ChangePassword(ChangePasswordDto changePassword, string Token)
        {
            try
            {
                _logger.LogInformation("User Want to Change Password ");
                //get user from token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(Token.Replace("Bearer ", ""));
                var userId = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new BadRequestException("User Not Found");
                }
               
                var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changePassword.OldPassword);
               
                if (!isOldPasswordValid)
                {
                    throw new BadRequestException("Old Password is Incorrect");
                }
                
                var result = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    throw new ValidationErrorsExecption(errors);
                }
               
                _logger.LogInformation("User with Email {0} Changed Password Successfully", user.Email);
               
                return new GenericResponseDto
                {
                    Success = true,
                    Message = "Password Changed Successfully",
                    StatusCode = 200,
                    Errors = null,
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Change Password ", ex.Message);
                return new  GenericResponseDto
                {
                    Errors = new List<string>() { ex.Message },
                    Message = "Error while Change Password",
                    StatusCode = 500,
                    Success = false,
                    Timestamp = DateTime.UtcNow
                };
            }
        }
       
        //forget password
        public async  Task<GenericResponseDto> ForgetPassword(ForgetPasswordDto forgetPassword)
        {
            try
            {
                _logger.LogInformation("User with email {0} Forget password", forgetPassword.Email);
                //check on Email 
                var user = await _userManager.FindByEmailAsync(forgetPassword.Email);

                if (user is not null)
                {
                    // Send Email To User
                    var subject = "Password Reset";
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var resetUrl = $"{_config["BaseUrl"]}/api/Atuh/ResetPassword?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(forgetPassword.Email)}";

                    var emailSent = await SendEmail(forgetPassword.Email, resetUrl, subject);
                    if (!emailSent)
                        throw new BadRequestException("Failed to send Reset Password  email.");

                }
                // will Not Check if User Exist To Avoid Hackers
                // Send Response
                return new GenericResponseDto() {
                    Message = "IF Email Exist Check Your Email To Rest Password.",
                    StatusCode = 200,
                    Errors=null,
                    Success=true,
                    Timestamp =DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Forget Password ", ex.Message);
                return new GenericResponseDto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = ex.Message,
                    Errors =null,
                    Timestamp =DateTime.UtcNow
                };

            }
        }

        //Rest password
        public async Task<GenericResponseDto> ResetPassword(ResetPasswordDto resetPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);

                if (user is null) throw new NotFoundException("Invalid User Email");

                var res = await _userManager.ResetPasswordAsync(user, resetPassword.ResetToken, resetPassword.NewPassword);
                if (!res.Succeeded)
                {
                    var errors = res.Errors.Select(e => e.Description);
                    throw new ValidationErrorsExecption(errors);
                }

                return new GenericResponseDto
                {
                    Success = true,
                    Message = "Reset Password",
                    StatusCode = 200,
                    Errors = null,
                    Timestamp = DateTime.UtcNow
                };

               

            }
            catch (Exception ex)
            {
                _logger.LogError("Erroe while Reset password for User", ex.Message);
                return new GenericResponseDto
                {
                    StatusCode = 400,
                    Success = false,
                    Errors = null,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                };
            }
            
        }

        //Doctor profile 
        public async Task<DoctorDetails> UpdateDoctorProfileAsync(string id, UpateDoctorDto doctorUpdateDto)
        {
            try
            {
                _logger.LogInformation("Doctor with Id {Id} want to Update His Data", id);

                if (id is null) throw new BadRequestException("Can not Enter Null value for doctor id");
               
                var doctor = await _doctorRepo.GetdoctorByUserIdAsync(id);
                if (doctor is null) throw new NotFoundException("Invalid doctor Data ");
                //check on phone 
                if (!String.IsNullOrEmpty(doctorUpdateDto.Phone1) && !Regex.IsMatch(doctorUpdateDto.Phone1, @"^(010|011|012|015)[0-9]{8}$"))
                    throw new BadRequestException($"Phone Number :{doctorUpdateDto.Phone1} is Invalid");
                if (!String.IsNullOrEmpty(doctorUpdateDto.Phone2) && !Regex.IsMatch(doctorUpdateDto.Phone2, @"^(010|011|012|015)[0-9]{8}$"))
                    throw new BadRequestException($"Phone Number :{doctorUpdateDto.Phone2} is Invalid");

                // Update Doctor Info

                doctor.ArbicName = doctorUpdateDto.arabicName ?? doctor.ArbicName;
                doctor.EnglishName = doctorUpdateDto.EnglishName ?? doctor.EnglishName;
                doctor.specialization = doctorUpdateDto.Specialization.ToString() ?? doctor.specialization;
                doctor.PhoneNumber1 = doctorUpdateDto.Phone1 ?? doctor.PhoneNumber1;
                doctor.PhoneNumber2 = doctorUpdateDto.Phone2 ?? doctor.PhoneNumber2;
                doctor.Bio = doctorUpdateDto.Bio ?? doctor.Bio;
                doctor.OpenHour = doctorUpdateDto.openingHours ?? doctor.OpenHour;
                doctor.CloseHour = doctorUpdateDto.closedHours ?? doctor.CloseHour;
                doctor.Address = doctorUpdateDto.Address ?? doctor.Address;

                
                //update Doctor
                _doctorRepo.Update(doctor);

                await _contexts.SaveChangesAsync();
                _logger.LogInformation("Doctor with User ID {0} Registered Successfully", doctor.UserId);

                return new DoctorDetails()
                {
                    UserId = doctor.UserId.ToString(),
                    UserName = doctor.User.UserName!,
                    Email = doctor.User.Email!,
                    arabicName = doctor.ArbicName!,
                    EnglishName = doctor.EnglishName!,
                    Specialization = doctor.specialization!,
                    Rating = doctor.Rate,
                    ProfilePictureUrl = doctor.User.ImageProfile!,
                    Bio = doctor.Bio!,
                    Phone1 = doctor.PhoneNumber1!,
                    Phone2 = doctor.PhoneNumber2!,
                    openingHours = doctor.OpenHour!,
                    closedHours = doctor.CloseHour!,
                    Address = doctor.Address!
                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Updating Doctor profile");
                throw;
            }
        }
     
        //Patient profile
        public async Task<PatientDetailsDto> UpdatePatientDetailsAsync(string Id, UpdatePatientDto patientDetailsDto)
        {
            try
            {
                _logger.LogInformation("patient with Id {Id} want to Update His Data", Id);

                if (Id is null) throw new BadRequestException("Can not Enter Null value for doctor id");

                var patient = await patientRepo.GetPatientByUserIdAsync(Id);
                if (patient is null) throw new NotFoundException("Invalid patient Data ");
                //check on phone 
                if (!String.IsNullOrEmpty(patientDetailsDto.Phone) && !Regex.IsMatch(patientDetailsDto.Phone, @"^(010|011|012|015)[0-9]{8}$"))
                    throw new BadRequestException($"Phone Number :{patientDetailsDto.Phone} is Invalid");

                // Update patient Info

                patient.ArabicName = patientDetailsDto.ArabicName ?? patient.ArabicName;
                patient.EnglishName = patientDetailsDto.EnglishName ?? patient.EnglishName;

                patient.Phone = patientDetailsDto.Phone ?? patient.Phone;
                patient.Bio = patientDetailsDto.Bio ?? patient.Bio;
                patient.City = patientDetailsDto.City ?? patient.City;
                patient.BirthDate = patientDetailsDto.BirthDate ?? patient.BirthDate;
                patient.Age = patientDetailsDto.BirthDate.HasValue ? DateTime.Now.Year - patientDetailsDto.BirthDate.Value.Year : patient.Age;

                //update patient
                 patientRepo.UpdatePatientProfileAsync(patient);

                await _contexts.SaveChangesAsync();
                _logger.LogInformation("patient with User ID {0} Registered Successfully", patient.UserId);

                return new PatientDetailsDto()
                {
                    UserId = patient.UserId.ToString(),
                    UserName = patient.User.UserName!,
                    Email = patient.User.Email!,
                    ArabicName = patient.ArabicName!,
                    EnglishName = patient.EnglishName!,
                    profileImage = patient.User.ImageProfile!,
                    Bio = patient.Bio!,
                    Phone = patient.Phone!,
                    City = patient.City!,
                    Age=patient.Age,
                    BirthDate=patient.BirthDate
                };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Updating Doctor profile");
                throw;
            }
        }
        // Update Profile Image
        public async Task<GenericResponseDto> UpdateProfileImage(UpdateProfileimage updateProfileimage, string Id, string Role)
        {
            try
            {
                _logger.LogInformation("User Want to Update Profile Image ");
                // Implementation for updating profile image goes here

                var user = await _userManager.FindByIdAsync(Id);
                var profilePath = string.Empty;
                if (Role == "Doctor")
                {
                    profilePath = "images/Doctorimages/Default_Icone.png";
                    if (updateProfileimage.ProfileImageUrl != null && updateProfileimage.ProfileImageUrl.Length > 0)
                        profilePath = await UploadImageAsync(updateProfileimage.ProfileImageUrl, "DoctorImages");
                }
                else
                {
                    profilePath = "images/patientimages/Default_Icone.png";
                    if (updateProfileimage.ProfileImageUrl != null && updateProfileimage.ProfileImageUrl.Length > 0)
                        profilePath = await UploadImageAsync(updateProfileimage.ProfileImageUrl, "patientImages");
                }
                user.ImageProfile = profilePath;
                var res = await _userManager.UpdateAsync(user);
                if (!res.Succeeded)
                {
                    var errors = res.Errors.Select(s => s.Description);
                    throw new ValidationErrorsExecption(errors);
                }

                return new GenericResponseDto
                {
                    StatusCode = 200,
                    Success = true,
                    Errors = null,
                    Message = "Upate Image Successfully",
                    Timestamp = DateTime.Now,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while Updating Profile Image");
                return new GenericResponseDto
                {
                    StatusCode = 400,
                    Success = false,
                    Errors = null,
                    Message = ex.Message,
                    Timestamp = DateTime.Now,
                };

            }
        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var jwtToken = _jwt.Value;
            List<Claim> userClaim = new List<Claim>()
            {
                new Claim (ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            //get user role 
            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                userClaim.Add(new Claim(ClaimTypes.Role, role));
            }
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtToken.SecretKey));
            var token = new JwtSecurityToken(
                 issuer: jwtToken.Issuer
                 , audience: jwtToken.Audience
                 , claims: userClaim
                 , expires: DateTime.UtcNow.AddDays(jwtToken.DurationDays)// Token will expire after 7 days
                 , signingCredentials: new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature)
                 );
            var ResultToken = new JwtSecurityTokenHandler().WriteToken(token);
            
            return ResultToken;
        }
        private async Task<string> UploadImageAsync(IFormFile imageFile,string PathFile)
        {
            var images = "images/Default_Icone.png";

            if (imageFile != null && imageFile.Length > 0)
            {
                var UploadImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine(UploadImage, fileName);

                using (var Stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(Stream);
                }
                images = $"/images/{PathFile}/{fileName}"; // Set the new profile picture path
            }
            return images;
        }
        private async Task<bool> SendEmail(string email, string body, string subject)
        {

            // Send Email Logic Here
            var emailSettings = new MailMessage();

            emailSettings.From = new MailAddress(_config["EmailSettings:From"]);
            emailSettings.To.Add(email);
            emailSettings.Subject = subject;
            emailSettings.Body = $"We received a request to reset your password. Click the link below to set a new one:\n" +
                $" {body}\n This link will expire in 1 hour \n" +
                $" If you did not request a password reset, please ignore this email.";

            using var smtp = new SmtpClient(_config["EmailSettings:SmtpHost"], int.Parse(_config["EmailSettings:SmtpPort"]));
            smtp.EnableSsl = bool.Parse(_config["EmailSettings:EnableSsl"]);
            smtp.Credentials = new NetworkCredential("mmmelkady23@gmail.com", _config["EmailSettings:Password"]);

            try
            {
                await smtp.SendMailAsync(emailSettings);
                return true;
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Failed to send  email. {ex}");
            }
            finally
            {
                emailSettings.Dispose(); // Dispose of the email message to free resources
            }

        }

       
    }

}
