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
        public async Task<ActionResult<ResultDTO<AuthResponse>>> LogIn(LoginRequest LoginData)
        {
                var Result = await authenticationService.LoginAsync(LoginData);
            if (!Result.Success)
                return Unauthorized(Result);
            return Result;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ResultDTO<RegistrationRequest>>> Register(RegistrationRequest registrationRequest)
        {

            var result = await authenticationService.RegisterAsync(registrationRequest);
                if(!result.Success)
                    return BadRequest(result);

            return result;
        }
  
        [HttpPost("VerifyEmailCode")]
        public async Task<ActionResult<ResultDTO<AuthResponse>>> VerifyEmailToken(ConfirmEmailVerificationRequest verificationToken)
        {
            var result = await authenticationService.VerifyEmailToken(verificationToken);
            if(!result.Success)
                    return BadRequest(result);
           return result;
        }


        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ResultDTO<Object>>> ForgotPassword(EmailVerificationRequest verifyToken)
        {
            if (await authenticationService.ForgetPasswordAsync(verifyToken))
                return new ResultDTO<Object>(true,"The password reset code has been sent to your mail.");
            return BadRequest(new ResultDTO<Object>(false, "Invalid Mail"));
        }

        [HttpPost("VerifyResetCode")]
        public async Task<ActionResult<ResultDTO<Object>>> VerifyResetToken(ConfirmEmailVerificationRequest verifyResetTokenDTO)
        {
            if (await authenticationService.VerifyTokenAsync(verifyResetTokenDTO)== null)
                    return BadRequest(new ResultDTO<Object>(false,"Invalid or expired OTP."));
                return new ResultDTO<Object>(true, "OTP verified successfully. You can now reset your password.");
        }
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResultDTO<Object>>> ResetPassword(ResetPasswordRequest resetPasswordDTO)
        {
            if (await authenticationService.ResetPasswordAsync(resetPasswordDTO))
                return new ResultDTO<Object>(true, "Password has been reset successfully.");
            return BadRequest(new ResultDTO<Object>(false, "Invalid or expired OTP."));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<ResultDTO<AuthResponse>>> RefreshToken(RefreshTokenRequest request)
        {
            var result = await authenticationService.RefreshTokenAsync(request);
            if (!result.Success)
                return Unauthorized(result);
            return result;
        }
      
    }
}
