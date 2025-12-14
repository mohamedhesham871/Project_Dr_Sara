using Project_Dr_Sara.Dtos;

namespace Project_Dr_Sara.Services.IServices
{
    public interface IAuthService
    {
         Task <UserResponse> Login(UserLogInRequest userLoginRequest);

         Task <UserResponse> RegisterDoctor(DoctorRegister DoctorRegisterRequest);

         Task <UserResponse> RegisterPatient(PatientRegister PatientRegisterRequest);

         Task<GenericResponseDto> ChangePassword(ChangePasswordDto changePassword, string Token);
         Task<GenericResponseDto> ForgetPassword(ForgetPasswordDto forgetPassword);
         Task<GenericResponseDto> ResetPassword(ResetPasswordDto resetPassword);


        Task<DoctorDetails> UpdateDoctorProfileAsync(string id, UpateDoctorDto doctorUpdateDto);
        Task<PatientDetailsDto> UpdatePatientDetailsAsync(string Id , UpdatePatientDto patientDetailsDto);
        Task<GenericResponseDto> UpdateProfileImage(UpdateProfileimage updateProfileimage, string Id, string Role);
       


    }
}
