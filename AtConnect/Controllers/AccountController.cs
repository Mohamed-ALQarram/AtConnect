using AtConnect.BLL.DTOs;
using AtConnect.BLL.Interfaces;
using AtConnect.Core.Models;
using AtConnect.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace AtConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AccountController(IAuthenticationService authentication)
        {
            this.authenticationService = authentication;
        }
       
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> LogIn(LoginRequest LoginData)
        {
                var Result = await authenticationService.LoginAsync(LoginData);
            if (!Result.Success)
                return Unauthorized(new ApiResponse<AuthResponse>(Result.Success,Result.ErrorMessage, Result.Data));
            return new ApiResponse<AuthResponse>(Result.Success, "Logged in Successfully.", Result.Data);
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponse<Object>>> Register(RegistrationRequest registrationRequest)
        {

            var result = await authenticationService.RegisterAsync(registrationRequest);
                if(!result.Success)
                    return BadRequest(new ApiResponse<object>(result.Success,result.ErrorMessage ,result.Data!));

            return new ApiResponse<Object>(result.Success,result.ErrorMessage, result.Data!);
        }
  
        [HttpPost("VerifyEmailToken")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> VerifyEmailToken(ConfirmEmailVerificationRequest verificationToken)
        {
            var result = await authenticationService.VerifyEmailToken(verificationToken);
            if(!result.Success)
                    return BadRequest(new ApiResponse<AuthResponse>(false,"Invalid Mail", null!));
           return Ok(new ApiResponse<AuthResponse>(result.Success, "Your account verified successfully.", result.Data));
        }


        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ApiResponse<Object>>> ForgotPassword(EmailVerificationRequest verifyToken)
        {
            if (await authenticationService.ForgetPasswordAsync(verifyToken))
                return new ApiResponse<Object>(true,"The password reset code has been sent to your mail.");
            return BadRequest(new ApiResponse<Object>(false, "Invalid Mail"));
        }

        [HttpPost("VerifyResetToken")]
        public async Task<ActionResult<ApiResponse<Object>>> VerifyResetToken(ConfirmEmailVerificationRequest verifyResetTokenDTO)
        {
            if (await authenticationService.VerifyTokenAsync(verifyResetTokenDTO)== null)
                    return BadRequest(new ApiResponse<Object>(false,"Invalid or expired OTP."));
                return new ApiResponse<Object>(true, "OTP verified successfully. You can now reset your password.");
        }
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ApiResponse<Object>>> ResetPassword(ResetPasswordRequest resetPasswordDTO)
        {
            if (await authenticationService.ResetPasswordAsync(resetPasswordDTO))
                return new ApiResponse<Object>(true, "Password has been reset successfully.");
            return BadRequest(new ApiResponse<Object>(false, "Invalid or expired OTP."));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            var result = await authenticationService.RefreshTokenAsync(request);
            if (!result.Success)
                return Unauthorized(new ApiResponse<AuthResponse>(result.Success, result.ErrorMessage, result.Data));
            return new ApiResponse<AuthResponse>(true, "Token has been refreshed successfully.", result.Data);
        }
      
    }
}
